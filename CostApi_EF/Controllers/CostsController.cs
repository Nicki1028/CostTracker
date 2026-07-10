using CostApi_EF.Models;
using CostApi_EF.Services;
using Microsoft.AspNetCore.Mvc;

namespace CostApi_EF.Controllers;

// 路由 /api/costs 對齊 Angular cost.service.ts 裡的 apiUrl = 'http://localhost:5000/api/costs'
[ApiController]
[Route("api/costs")]
public class CostsController : ControllerBase
{
    private readonly ICostService _costService;

    public CostsController(ICostService costService)
    {
        _costService = costService;
    }

    // 對應 CostService.getCosts(start, end, incomexpense, item, paymethod)
    [HttpGet]
    public ActionResult<List<CostItem>> Get(
        [FromQuery] string start,
        [FromQuery] string end,
        [FromQuery] string incomexpense = "全部",
        [FromQuery] string item = "全部",
        [FromQuery] string paymethod = "全部")
    {
        if (!DateOnly.TryParse(start, out var startDate) || !DateOnly.TryParse(end, out var endDate))
        {
            return BadRequest("日期格式錯誤，請使用 yyyy-MM-dd");
        }

        var result = _costService.GetCosts(startDate, endDate, incomexpense, item, paymethod);
        return Ok(result);
    }

    // 對應 CostService.addCost(cost)
    [HttpPost]
    public IActionResult Post([FromBody] CostItem item)
    {
        _costService.AddCost(item);
        // Angular 那邊呼叫的是 addCost(): Observable<void>，
        // 回傳 204 No Content 比回傳 200 帶空 body 更安全，避免前端 JSON 解析報錯。
        return NoContent();
    }
}
