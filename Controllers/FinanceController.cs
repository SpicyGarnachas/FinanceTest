using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Linq;

namespace FinanceTest.Controllers;

[ApiController]
[Route("[controller]")]
public class FinanceController : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, Portfolio> _portfolios = new();

    // --- AUTH TEST ---
    [HttpGet]
    [Authorize]
    public IActionResult GetUser()
    {
        var username = User.Identity?.Name ?? "Unknown";
        return Ok($"✅ Access granted to {username}");
    }

    // --- PORTFOLIO CRUD ---
    [HttpGet("portfolios")]
    [Authorize]
    public IActionResult GetAll() => Ok(_portfolios.Values);

    [HttpGet("portfolios/{id}")]
    [Authorize]
    public IActionResult GetById(Guid id)
    {
        if (_portfolios.TryGetValue(id, out var portfolio))
            return Ok(portfolio);
        return NotFound();
    }

    [HttpPost("portfolios")]
    [Authorize]
    public IActionResult Create([FromBody] Portfolio portfolio)
    {
        portfolio.Id = Guid.NewGuid();
        _portfolios[portfolio.Id] = portfolio;
        return CreatedAtAction(nameof(GetById), new { id = portfolio.Id }, portfolio);
    }

    [HttpPut("portfolios/{id}")]
    [Authorize]
    public IActionResult Update(Guid id, [FromBody] Portfolio updated)
    {
        if (!_portfolios.ContainsKey(id))
            return NotFound();

        updated.Id = id;
        _portfolios[id] = updated;
        return NoContent();
    }

    [HttpDelete("portfolios/{id}")]
    [Authorize]
    public IActionResult Delete(Guid id)
    {
        if (_portfolios.TryRemove(id, out _))
            return NoContent();
        return NotFound();
    }

    // --- EXPENSES ---
    [HttpPost("portfolios/{portfolioId}/expenses")]
    [Authorize]
    public IActionResult AddExpense(Guid portfolioId, [FromBody] Expense expense)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            expense.Id = Guid.NewGuid();
            portfolio.Expenses.Add(expense);
            return Ok(expense);
        }
        return NotFound();
    }

    [HttpPut("portfolios/{portfolioId}/expenses/{expenseId}")]
    [Authorize]
    public IActionResult UpdateExpense(Guid portfolioId, Guid expenseId, [FromBody] Expense updated)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var expense = portfolio.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expense == null) return NotFound();

            expense.Name = updated.Name;
            expense.Amount = updated.Amount;
            return Ok(expense);
        }
        return NotFound();
    }

    [HttpDelete("portfolios/{portfolioId}/expenses/{expenseId}")]
    [Authorize]
    public IActionResult DeleteExpense(Guid portfolioId, Guid expenseId)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var removed = portfolio.Expenses.RemoveAll(e => e.Id == expenseId);
            return removed > 0 ? NoContent() : NotFound();
        }
        return NotFound();
    }

    // --- INCOMES ---
    [HttpPost("portfolios/{portfolioId}/incomes")]
    [Authorize]
    public IActionResult AddIncome(Guid portfolioId, [FromBody] Income income)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            income.Id = Guid.NewGuid();
            portfolio.Incomes.Add(income);
            return Ok(income);
        }
        return NotFound();
    }

    [HttpPut("portfolios/{portfolioId}/incomes/{incomeId}")]
    [Authorize]
    public IActionResult UpdateIncome(Guid portfolioId, Guid incomeId, [FromBody] Income updated)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var income = portfolio.Incomes.FirstOrDefault(i => i.Id == incomeId);
            if (income == null) return NotFound();

            income.Source = updated.Source;
            income.Amount = updated.Amount;
            return Ok(income);
        }
        return NotFound();
    }

    [HttpDelete("portfolios/{portfolioId}/incomes/{incomeId}")]
    [Authorize]
    public IActionResult DeleteIncome(Guid portfolioId, Guid incomeId)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var removed = portfolio.Incomes.RemoveAll(i => i.Id == incomeId);
            return removed > 0 ? NoContent() : NotFound();
        }
        return NotFound();
    }

    // --- INVESTMENTS ---
    [HttpPost("portfolios/{portfolioId}/investments")]
    [Authorize]
    public IActionResult AddInvestment(Guid portfolioId, [FromBody] Investment investment)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            investment.Id = Guid.NewGuid();
            portfolio.Investments.Add(investment);
            return Ok(investment);
        }
        return NotFound();
    }

    [HttpPut("portfolios/{portfolioId}/investments/{investmentId}")]
    [Authorize]
    public IActionResult UpdateInvestment(Guid portfolioId, Guid investmentId, [FromBody] Investment updated)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var investment = portfolio.Investments.FirstOrDefault(i => i.Id == investmentId);
            if (investment == null) return NotFound();

            investment.Asset = updated.Asset;
            investment.Value = updated.Value;
            return Ok(investment);
        }
        return NotFound();
    }

    [HttpDelete("portfolios/{portfolioId}/investments/{investmentId}")]
    [Authorize]
    public IActionResult DeleteInvestment(Guid portfolioId, Guid investmentId)
    {
        if (_portfolios.TryGetValue(portfolioId, out var portfolio))
        {
            var removed = portfolio.Investments.RemoveAll(i => i.Id == investmentId);
            return removed > 0 ? NoContent() : NotFound();
        }
        return NotFound();
    }
}

public class Portfolio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Currency { get; set; } = "USD";
    public List<Expense> Expenses { get; set; } = new();
    public List<Investment> Investments { get; set; } = new();
    public List<Income> Incomes { get; set; } = new();
}

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal Amount { get; set; }
}

public class Investment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Asset { get; set; }
    public decimal Value { get; set; }
}

public class Income
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Source { get; set; }
    public decimal Amount { get; set; }
}
