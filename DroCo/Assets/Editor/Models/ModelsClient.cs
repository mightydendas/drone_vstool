using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroCo.Editor {
    public class ModelsClient {
        public static async Task<List<ModelListDto>> GetAll() {
            Message request = new Message() {
                Type = "model_get_all",
                Data = null,
            };

            Message<List<ModelListDto>> response = await HttpClient.SendRequest<List<ModelListDto>>(request);

            if (response == null) {
                return null;
            }

            return response.Data;
        }

        public static async Task<ModelDto> Get(int id) {
            Message<int> request = new Message<int>() {
                Type = "model_get",
                Data = id,
            };

            Message<ModelDto> response = await HttpClient.SendRequest<int, ModelDto>(request);

            if (response == null) {
                return null;
            }

            return response.Data;
        }

        public static async Task<int> ModelCreate(ModelCreateDto dto) {
            Message<ModelCreateDto> request = new Message<ModelCreateDto>() {
                Type = "model_create",
                Data = dto,
            };

            Message<int> response = await HttpClient.SendRequest<ModelCreateDto, int>(request);

            if (response == null) {
                return -1;
            }

            return response.Data;
        }

        public static async Task<bool> ModelDelete(int id) {
            Message<int> request = new Message<int>() {
                Type = "model_delete",
                Data = id,
            };

            Message<bool> response = await HttpClient.SendRequest<int, bool>(request);

            if (response == null) {
                return false;
            }

            return response.Data;
        }
    }
}
