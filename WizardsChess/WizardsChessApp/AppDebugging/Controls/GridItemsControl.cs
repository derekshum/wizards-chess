using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WizardsChess.AppDebugging.Controls
{
	public class GridItemsControl : ItemsControl
	{
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			var frameworkElmt = element as FrameworkElement;
			if (frameworkElmt != null)
			{
				Binding rowBinding = new Binding();
				rowBinding.Path = new PropertyPath("GridRow");
				rowBinding.Source = item;
				rowBinding.Mode = BindingMode.OneWay;
				frameworkElmt.SetBinding(Grid.RowProperty, rowBinding);
				Binding colBinding = new Binding();
				colBinding.Path = new PropertyPath("GridColumn");
				colBinding.Source = item;
				colBinding.Mode = BindingMode.OneWay;
				frameworkElmt.SetBinding(Grid.ColumnProperty, colBinding);
			}
		}
	}
}
