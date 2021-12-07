﻿using Corporate_messenger.DB.Repository;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "Messanger.db";
        public static ChatListRepository database;
        public static ChatListRepository Database
        {
            get
            {
                if (database == null)
                {

                    database = new ChatListRepository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();


            var token = DependencyService.Get<IFileService>().ReadFile(DependencyService.Get<IFileService>().GetRootPath());
            var data = token.Split('/');

             if (data[0] != "")
              {
                  SpecialDataModel special = new SpecialDataModel();
                  special.Token = data[0];
                  special.Id = Int32.Parse(data[1]);
                  special.Name = data[2];
                  MainPage = new AuthorizationMainPage();
              }
              else
              {
                MainPage = new NavigationPage(new LoginPage());
                // MainPage = new NavigationPage(new AuthorizationMainPage());
            }

          // MainPage = new AuthorizationPage();



        }

        protected override void OnStart()
        {
            var file = DependencyService.Get<IFileService>().CreateFile();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
