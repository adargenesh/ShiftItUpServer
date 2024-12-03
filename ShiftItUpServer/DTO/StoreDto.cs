namespace ShiftItUpServer.DTO
{
    public class StoreDto
    {
      
            public int IdStore { get; set; }

            public string StoreName { get; set; } = string.Empty;

        public string StoreAddress { get; set; } = null;  

            public string StoreManager { get; set; } = null;

        public string ManagerEmail { get; set; } = null;
        public string ProfileImagePath { get; set; } = "";
        public StoreDto(Models.Store modelStore)
        {
            this.IdStore = modelStore.IdStore;
            this.StoreName = modelStore.StoreName;
            this.StoreAddress=modelStore.StoreAdress;
            this.StoreManager = modelStore.StoreManager;
            this.ManagerEmail = modelStore.ManagerEmail;

        }
        public Models.Store GetModel()
        {
            Models.Store model = new Models.Store();
            model.IdStore = this.IdStore;
            model.StoreName = this.StoreName;
            model.StoreAdress=this.StoreAddress;
            model.StoreManager = this.StoreManager;
            model.ManagerEmail = this.ManagerEmail;
            return model;
        }
      

    }
}
