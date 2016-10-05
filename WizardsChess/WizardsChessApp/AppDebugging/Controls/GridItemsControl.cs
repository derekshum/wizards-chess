using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WizardsChessApp.AppDebugging.Controls
{
	public class GridItemsControl : ItemsControl
	{
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			System.Diagnostics.Debug.WriteLine($"Called IsItemItsOwnContainerOverride with item {item?.ToString()}");
			var result = base.IsItemItsOwnContainerOverride(item);
			System.Diagnostics.Debug.WriteLine($"IsItemOwnContainer returning {result}");
			return result;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			System.Diagnostics.Debug.WriteLine("Called GetContainerForItemOverride");
			var result = base.GetContainerForItemOverride();
			System.Diagnostics.Debug.WriteLine("Finished GetContainerForItemOverride");
			return result;
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			System.Diagnostics.Debug.WriteLine("Called PrepareContainerForItemOverride");

			base.PrepareContainerForItemOverride(element, item);

			var frameworkElmt = element as FrameworkElement;
			if (frameworkElmt != null)
			{
				var style = new Style(typeof(ContentPresenter));
				if (frameworkElmt.Style?.Setters != null)
				{
					foreach(var setter in frameworkElmt.Style.Setters)
					{
						style.Setters.Add(setter);
					}
				}
				style.Setters.Add(new Setter(Grid.RowProperty, (item as ObservableChessPiece).GridRow));
				style.Setters.Add(new Setter(Grid.ColumnProperty, (item as ObservableChessPiece).GridColumn));
				frameworkElmt.Style = style;
			}

			System.Diagnostics.Debug.WriteLine("Completed PrepareContainerForItemOverride");
		}
	}
}
