using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ConceptTest
{
    public class DynamicJsonTest
    {
        [Fact]
        public void AddDynamicJsonProperty()
        {
            var dummy = new DummyJsonModel
            {
                Name = "name"
            };

            string valueString = JsonConvert.SerializeObject(dummy);
            dynamic model = JsonConvert.DeserializeObject(valueString);

            model.Id = "id";
            model.Dummy = "dummy";

            valueString = JsonConvert.SerializeObject(model);
            
            Assert.True(true);
        }
        
        [Fact]
        public void AddTreeJson()
        {
            var tree = new Tree
            {
                NodeA = new NodeA()
                {
                    A1 = "a1",
                    A2 = "a2"
                },
                NodeB = new NodeB()
                {
                    B1 = "b1",
                    B2 = "b2"
                }
            };
            
            string valueString = JsonConvert.SerializeObject(tree);

            Tree greenTree = JsonConvert.DeserializeObject<Tree>(valueString);

            var aNode = greenTree.NodeA as dynamic;

            var a = ((JObject) aNode) .ToObject<NodeA>();
            string value1 = a.A1;
            string value2 = aNode.A2;

        }
    }
    
    
}