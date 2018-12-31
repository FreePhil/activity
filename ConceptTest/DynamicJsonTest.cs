using Newtonsoft.Json;
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
    }
}