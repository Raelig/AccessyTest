using Acessy.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acessy.Controllers
{
    [Route("operation")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private static Uri _BASE_LASAB_API_URL = new Uri("https://test.api.accessy.app/lasab");
        private static List<Operation> _FAKE_DB_LIST = new();

        // GET /operation>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_FAKE_DB_LIST);
        }

        // POST /operation
        [HttpPost]
        public ObjectResult Post([FromBody]Operation operation)
        {
            Guid guid = Guid.NewGuid();
            operation.Id = guid;
            _FAKE_DB_LIST.Add(operation);

            return Ok(System.Text.Json.JsonSerializer.Serialize(operation.Id));
        }

        //PUT /operation/{id}/invoke
        [HttpPut("/{id}/invoke")]
        public async Task<IActionResult> PutAsync([FromRoute] string id)
        {
                Token token = new Token();
                var user = new User()
                {
                    username = "Hello",
                    password = "Accessy!"
                };

                var userJson = JsonConvert.SerializeObject(user);
                var tokenPayload = new StringContent(userJson, Encoding.UTF8, "application/json");
                Lock lockToOpen = null;

            using (var client = new HttpClient())
            {
                //POST user to access token from LåsAB
                var tokenResponse = await client.PostAsync(_BASE_LASAB_API_URL + "/login", tokenPayload);

                if (tokenResponse.StatusCode == System.Net.HttpStatusCode.OK) //200
                {
                    var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<Token>(tokenResult);

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.token);

                    //GET all locks available
                    var lockResponse = await client.GetAsync(_BASE_LASAB_API_URL + "/lock");
                    List<Lock> lockList = new List<Lock>();


                    if (lockResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var lockResult = await lockResponse.Content.ReadAsStringAsync();
                        lockList = JsonConvert.DeserializeObject<List<Lock>>(lockResult);

                        lockToOpen = lockList.Find(lockToFind => lockToFind.id.Equals(id));
                        if (lockToOpen != null)
                        {
                            //PUT unlock the item with corresponding id
                            var unlockResponse = await client.PutAsync(_BASE_LASAB_API_URL + "/lock/" + id + "/unlock", null);
                            if (unlockResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                return Ok(); //200
                            }
                        }
                    }
                }  

            } if (lockToOpen == null)
            {
                return NotFound();
            }
            else
            {
                HttpContext.Response.Headers.Add("Description", "Failed to invoke operation");
                return BadRequest();
            }
        }
    }
}
