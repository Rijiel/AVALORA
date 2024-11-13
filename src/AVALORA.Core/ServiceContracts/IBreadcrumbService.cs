using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Core.ServiceContracts;

public interface IBreadcrumbService
{
    /// <summary>
    /// Sets custom breadcrumb nodes for the given controller.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="controllerActions">A list of controller actions.</param>
    /// <param name="titles">A list of titles corresponding to the controller actions.</param>
    /// <remarks>
    /// This method sets custom breadcrumb nodes for the given controller.
    /// It requires at least one action and one title to be provided.
    /// The number of actions and titles must be the same.
    /// </remarks>
    void SetCustomNodes(Controller controller, string controllerName, List<string> controllerActions, List<string> titles);

    /// <summary>
    /// Sets custom breadcrumb nodes for the given controller with route values.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="controllerActions">A list of controller actions.</param>
    /// <param name="titles">A list of titles corresponding to the controller actions.</param>
    /// <param name="routeValues">A list of dictionaries containing route values for each action.</param>
    /// <remarks>
    /// This method sets custom breadcrumb nodes for the given controller.
    /// It requires at least one action and one title to be provided.
    /// The number of actions and titles must be the same.
    /// The routeValues parameter will be used to specify additional route values for each action.
    /// </remarks>
    void SetCustomNodes(Controller controller, string controllerName, List<string> controllerActions,
        List<string> titles, List<Dictionary<string, object>> routeValues);
}
