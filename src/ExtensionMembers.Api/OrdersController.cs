using Microsoft.AspNetCore.Mvc;

namespace ExtensionMembers.Api;

[ApiController]
[Produces("application/json")]
[Route("api/orders")]
public sealed class OrdersController(OrderStore store) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<OrderSummary[]>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<OrderSummary>> GetAll() =>
        Ok(store.GetAll().Select(order => order.ToSummary()).ToArray());

    [HttpGet("{id:int}")]
    [ProducesResponseType<OrderSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<OrderSummary> GetById(int id)
    {
        var order = store.Find(id);
        return order is null
            ? Problem(title: "Order not found", statusCode: StatusCodes.Status404NotFound)
            : Ok(order.ToSummary());
    }

    [HttpGet("high-value")]
    [ProducesResponseType<OrderSummary[]>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<OrderSummary>> GetHighValue() =>
        Ok(store.GetAll().Where(order => order.IsHighValue).Select(order => order.ToSummary()).ToArray());

    [HttpGet("insights")]
    [ProducesResponseType<OrderInsights>(StatusCodes.Status200OK)]
    public ActionResult<OrderInsights> GetInsights()
    {
        var orders = store.GetAll();
        var insights = new OrderInsights(
            Order.HighValueThreshold,
            orders.Count,
            orders.Count(order => order.IsHighValue),
            orders.IsEmpty);

        return Ok(insights);
    }

    [HttpPost]
    [ProducesResponseType<OrderSummary>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<OrderSummary> Create(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            ModelState.AddModelError("customerName", "Customer name is required.");
        }
        else if (request.CustomerName.Trim().Length > 100)
        {
            ModelState.AddModelError("customerName", "Customer name cannot exceed 100 characters.");
        }

        if (request.TotalAmount < 0)
        {
            ModelState.AddModelError("totalAmount", "Total amount cannot be negative.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var order = store.Add(request.CustomerName!.Trim(), request.TotalAmount);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.ToSummary());
    }
}
