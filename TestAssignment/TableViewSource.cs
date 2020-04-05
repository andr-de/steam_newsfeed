using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace TestAssignment
{
	public class TableViewSource : UITableViewSource
	{
		List<string> tabledata;

		public TableViewSource(List<string> items)
		{
			tabledata = items;

		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("cells");

			cell.TextLabel.Text = tabledata[indexPath.Row];
			return cell;
		}



		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return tabledata.Count;
		}
	}
}