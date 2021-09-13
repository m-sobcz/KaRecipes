using KaRecipes.BL.RecipeAggregate;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaRecipes.UI.ViewModels
{
    public class ViewModelLocator
    {
        IServiceProvider serviceProvider;
        public ViewModelLocator()
        {
            serviceProvider = App.ServiceProvider;
        }
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<MainWindowViewModel>();
            }
        }
        public RecipesTabViewModel RecipesTabViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<RecipesTabViewModel>();
            }
        }

    }
}
