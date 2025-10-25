using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using RedBlackTree2.Models;
using JsonTree;
using RedBlackTree2.Extensions; 

namespace RedBlackTree2.Services
{
    public class RedBlackTreeService
    {
        private RedBlackTree<string> _tree = new RedBlackTree<string>();
        private readonly RedBlackTreeSerializer _treeserializer = new RedBlackTreeSerializer();
        private readonly ISession _session;

        public RedBlackTreeService(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
            EnsureUserGuidExists(); // Ստեղծում ենք Unique ID ըստ session-ի
            _tree = LoadTreeFromSession() ?? new RedBlackTree<string>();
        }
        private void EnsureUserGuidExists()
        {
            
            if (string.IsNullOrEmpty(_session.GetString("UserGuid")))
            {
                var newGuid = Guid.NewGuid().ToString();
                Console.WriteLine("Creating new user guid: " + newGuid);
                _session.SetString("UserGuid", newGuid);
            }
        }

        private string GetUserKey()
        {
            var guid = _session.GetString("UserGuid");
            return $"UserTree_{guid}";
        }

        private RedBlackTree<string> LoadTreeFromSession()
        {
            var key = GetUserKey();
            return _session.GetObject(key);
        }

        private void SaveTreeToSession()
        {
            var key = GetUserKey();
            _session.SetObject(key, _tree, _tree.NewValue);
        }

        public TreeViewModel GetTreeData()
        {
            var model = TreeViewModel.FromRBTree(_tree);
            model.InputValue = _tree.NewValue;
            model.isSearchClicked = _tree.isSearchClicked;
            model.Quantity = _tree.Quantity;
            model.InsertSteps = _tree.InsertSteps;
            return model;
        }

        public void InsertNode(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _tree.Insert(value);
                SaveTreeToSession();
            }
           
        }

        public (bool Success, string Message) DeleteNode(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (_tree.Delete(value))
                {
                    SaveTreeToSession();
                    return (true, $"{value}{(char.IsDigit(value[^1]) ? " -ը" : " -ն")} հեռացվել է։");
                }
                SaveTreeToSession();
                return (false, $"{value}{(char.IsDigit(value[^1]) ? " -ը" : " -ն")} չի հայտնաբերվել։");
            }

            return (false, "");
        }

        public (bool Success, string Message) SearchNode(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var node = _tree.Search(value);
                if (node != RedBlackTree<string>.NIL)
                {
                    SaveTreeToSession();
                    return (true, $"{value}{(char.IsDigit(value[^1]) ? " -ը" : " -ն")} հայտնաբերվել է։");
                }

                _tree.isSearchClicked = false;
                SaveTreeToSession();
                return (false, $"{value}{(char.IsDigit(value[^1]) ? " -ը" : " -ն")} չի հայտնաբերվել։");
            }
            else
            {
                return (false, "");
            }
        }

        public void ClearTree()
        {
            _tree = new RedBlackTree<string>();
            if (File.Exists($"Files/{_session.GetString("UserGuid")}.JSON"))
            {
                File.Delete($"Files/{_session.GetString("UserGuid")}.JSON");
            }
            SaveTreeToSession();
        }

        public string GetTreeStatistics()
        {
            if (_tree.Quantity <= 1) return null;
            _tree.isSearchClicked = false;
            SaveTreeToSession();
            return $"Հանգույցների քանակ` {_tree.Quantity}, " +
                   $"մինիմալ արժեք` {_tree.Minimum(_tree.Root).Value}, " +
                   $"մաքսիմալ արժեք` {_tree.Maximum(_tree.Root).Value}, " +
                   $"բարձրություն` {_tree.GetHeight(_tree.Root)}։";
        }

        public (byte[] Data, string FileName, string Error) ExportTree()
        {
            if (_tree.Root == RedBlackTree<string>.NIL)
                return (null, null, "Ծառը դատարկ է:");

            byte[] json = _treeserializer.SerializeObject(_tree, _tree.NewValue);
            return (json, $"RedBlackTree_{DateTime.Now:g}.JSON", null);
        }

        public string ImportTree(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return "Ֆայլը ընտրված չէ";
                
                if (!Path.GetExtension(file.FileName).Equals(".JSON", StringComparison.OrdinalIgnoreCase))
                    return "Միայն JSON ֆայլեր են թույլատրված։";

                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                
                string jsonContent = Encoding.UTF8.GetString(fileBytes);
                if (string.IsNullOrWhiteSpace(jsonContent))
                    return "Ֆայլը դատարկ է։";

                JsonNode.Parse(jsonContent);
                _tree = _treeserializer.DeserializeObject(fileBytes);
                SaveTreeToSession();
                return null;
            }
            catch (JsonException ex)
            {
                return $"Անվավեր JSON ֆորմատ։ {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Սխալ։ {ex.Message}";
            }
        }
    }
}
