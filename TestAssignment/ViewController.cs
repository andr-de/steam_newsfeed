using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TestAssignment
{
    public partial class ViewController : UIViewController
    {
        public List<string> Ids { get; set; }
        public List<News> GamesNews { get; set; }

        protected ViewController(IntPtr handle) : base(handle)
        {
            GamesNews = new List<News>();
            Ids = new List<string>();
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ids.json");
            if (File.Exists(fileName))
            {
                string data = File.ReadAllText(fileName);
                Ids = JsonConvert.DeserializeObject<List<string>>(data);
            }
        }

        

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mainTableView.RegisterClassForCellReuse(typeof(UITableViewCell), "cells");
            mainTableView.Source = new TableViewSource(Ids);
            mainTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
            mainTableView.ReloadData();

            DeleteIdButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                string curr_id = IDText.Text;
                if (!Ids.Contains(curr_id))
                {
                    var alert = UIAlertController.Create("Error", "This ID wasn't added", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                    IDText.ResignFirstResponder();
                }
                else
                {
                    Ids.Remove(curr_id);
                    GamesNews.RemoveAll(x => x.appid == curr_id);
                    IDText.Text = "";
                    IDText.ResignFirstResponder();
                    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ids.json");
                    var js = JsonConvert.SerializeObject(Ids);
                    File.WriteAllText(fileName, js);
                }
                mainTableView.Source = new TableViewSource(Ids);
                mainTableView.ReloadData();
            };

            AddNewIdButton.TouchUpInside += async (object sender, EventArgs e) =>
            {
                string curr_id = IDText.Text;
                if (Ids.Contains(curr_id))
                {
                    var alert = UIAlertController.Create("Error", "Already added", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                    IDText.ResignFirstResponder();
                    return;
                }

                if (curr_id == "")
                {
                    var alert = UIAlertController.Create("Error", "ID couldn't be empty", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                    IDText.ResignFirstResponder();
                    return;
                }
                
                int num;
                if (!int.TryParse(curr_id, out num))
                {
                    var alert = UIAlertController.Create("Error", "ID have to be an integer", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                    IDText.ResignFirstResponder();
                    return;
                } 

                var content = await FeedLoader.LoadData(IDText.Text);
                if (content == "internalError")
                {
                    var alert = UIAlertController.Create("Error", "Check your internet connection. ID wasn't added", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                    return;
                }

                if (content == "{}")
                {
                    var alert = UIAlertController.Create("Error", "Game with this ID doesn't exist according to API", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                }
                else
                {
                    Ids.Add(IDText.Text);
                    IDText.Text = "";
                    IDText.ResignFirstResponder();
                    GamesNews = await FeedLoader.LoadFeed(Ids);
                    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ids.json");
                    var js = JsonConvert.SerializeObject(Ids);
                    File.WriteAllText(fileName, js);
                    mainTableView.Source = new TableViewSource(Ids);
                    mainTableView.ReloadData();
                }
            };
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            GamesNews = await FeedLoader.LoadFeed(Ids);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            var newsFeedController = segue.DestinationViewController as NewsFeedController;

            if (newsFeedController != null)
            {
                newsFeedController.NewsFeed = GamesNews;
                newsFeedController.Ids = Ids;
            }
        }
    }
}