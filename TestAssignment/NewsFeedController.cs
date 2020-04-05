using System;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace TestAssignment
{
    public partial class NewsFeedController : UITableViewController
    {
        public List<News> NewsFeed { get; set; }
        public List<string> Ids { get; set; }

        static NSString newsCellId = new NSString("NewsCell");

        public NewsFeedController(IntPtr handle) : base(handle)
        {
            TableView.Source = new NewsFeedDataSource(this);
            NewsFeed = new List<News>();
            NewsFeedTable.TableFooterView = new UIView();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateButton.TouchUpInside += async (object sender, EventArgs e) => {

                NewsFeed = await FeedLoader.LoadFeed(Ids);
                NewsFeedTable.ReloadData();
            };
        }

        class NewsFeedDataSource : UITableViewSource
        {
            NewsFeedController controller;
            string cellIdentifier = "NewsCell";

            public NewsFeedDataSource(NewsFeedController controller)
            {
                this.controller = controller;
            }

            public override nint RowsInSection(UITableView tableView, nint section)
            {
                return controller.NewsFeed.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                NewsCell cell = (NewsCell)tableView.DequeueReusableCell(cellIdentifier);
                int row = indexPath.Row;
                cell.UpdateCell(controller.NewsFeed[row]);

                return cell;
            }
        }
    }
}