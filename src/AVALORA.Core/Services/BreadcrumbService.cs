using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Nodes;

namespace AVALORA.Core.Services;

public class BreadcrumbService : IBreadcrumbService
{
    public void SetCustomNodes(Controller controller, string controllerName, List<string> controllerActions, 
        List<string> titles)
    {
        if (controllerActions.Count == 0 || titles.Count == 0)
        {
            throw new ArgumentException("At least one action and one title must be provided.");
        }

        if (controllerActions.Count != titles.Count)
        {
            throw new ArgumentException("The number of actions and titles must be the same.");
        }

        controller.ControllerContext.ActionDescriptor.RouteValues.TryGetValue("area", out string? areaName);

        List<MvcBreadcrumbNode> nodes = [];

        // Create breadcrumb node for each action
        for (int i = 0; i < titles.Count; i++)
        {
            var node = new MvcBreadcrumbNode(action: controllerActions[i], controller: controllerName, title: titles[i], 
                areaName: areaName)
            {
                OverwriteTitleOnExactMatch = true,
                Parent = i == 0 ? null : nodes[i - 1]
            };

            nodes.Add(node);
        }

        // Set the controller's breadcrumb node through ViewData
        controller.ViewData["BreadcrumbNode"] = nodes[^1];
    }

    public void SetCustomNodes(Controller controller, string controllerName, List<string> controllerActions,
        List<string> titles, List<Dictionary<string, object>> routeValues)
    {
        if (controllerActions.Count == 0 || titles.Count == 0)
        {
            throw new ArgumentException("At least one action and one title must be provided.");
        }

        if (controllerActions.Count != titles.Count)
        {
            throw new ArgumentException("The number of actions and titles must be the same.");
        }

        controller.ControllerContext.ActionDescriptor.RouteValues.TryGetValue("area", out string? areaName);

        List<MvcBreadcrumbNode> nodes = [];

        // Create breadcrumb node for each action
        for (int i = 0; i < titles.Count; i++)
        {
            var node = new MvcBreadcrumbNode(action: controllerActions[i], controller: controllerName, title: titles[i],
                areaName: areaName)
            {
                OverwriteTitleOnExactMatch = true,
                Parent = i == 0 ? null : nodes[i - 1]
            };

            if (routeValues.Count > i)
            {
                if (routeValues[i] != null)
                    node.RouteValues = routeValues[i];
            }

            nodes.Add(node);
        }

        // Set the controller's breadcrumb node through ViewData
        controller.ViewData["BreadcrumbNode"] = nodes[^1];
    }
}
