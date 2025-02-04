﻿namespace ShiftItUpServer.DTO
{
    public class WorkerDto
    {



        public WorkerDto()
        {

        }
        
            public int WorkerId { get; set; }

            public string UserName { get; set; } = null;

            public string UserLastName { get; set; } = null;

            public string UserEmail { get; set; } = null;

            
            public string  UserSalary { get; set; }  // Changed to decimal for better salary representation

            public int StatusWorker { get; set; }
        public int IdStore { get; set; } 
        public string UserPassword { get; set; } = null;
        public string ProfileImagePath { get; set; } = "";


        public WorkerDto(Models.Worker modelWorker)
        {
            this.WorkerId = modelWorker.WorkerId;
            this.UserName = modelWorker.UserName;
            this.UserLastName = modelWorker.UserLastName;
            this.UserEmail = modelWorker.UserEmail;
            this.UserSalary = modelWorker.UserSalary;
            this.StatusWorker = modelWorker.StatusWorker;
            this.IdStore = modelWorker.IdStore;
            this.UserPassword = modelWorker.UserPassword;
        }
        public Models.Worker GetModel()
        {
            Models.Worker model = new Models.Worker();
            model.WorkerId = this.WorkerId;
            model.UserName = this.UserName;
            model.UserLastName = this.UserLastName;
            model.UserEmail = this.UserEmail;
            model.UserSalary = this.UserSalary;
            model.StatusWorker = this.StatusWorker;
            model.IdStore = this.IdStore;
            model.UserPassword = this.UserPassword;

            return model;
        }


    }
}
