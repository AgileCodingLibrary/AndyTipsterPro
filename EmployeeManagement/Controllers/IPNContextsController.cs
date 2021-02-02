using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;

namespace EmployeeManagement.Controllers
{
    public class IPNContextsController : Controller
    {
        private readonly AppDbContext _context;

        public IPNContextsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: IPNContexts
        public async Task<IActionResult> Index()
        {
            return View(await _context.IPNContexts.ToListAsync());
        }

        // GET: IPNContexts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iPNContext = await _context.IPNContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iPNContext == null)
            {
                return NotFound();
            }

            return View(iPNContext);
        }

        // GET: IPNContexts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IPNContexts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RequestBody,Verification")] IPNContext iPNContext)
        {
            if (ModelState.IsValid)
            {
                _context.Add(iPNContext);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(iPNContext);
        }

        // GET: IPNContexts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iPNContext = await _context.IPNContexts.FindAsync(id);
            if (iPNContext == null)
            {
                return NotFound();
            }
            return View(iPNContext);
        }

        // POST: IPNContexts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequestBody,Verification")] IPNContext iPNContext)
        {
            if (id != iPNContext.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iPNContext);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IPNContextExists(iPNContext.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(iPNContext);
        }

        // GET: IPNContexts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iPNContext = await _context.IPNContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (iPNContext == null)
            {
                return NotFound();
            }

            return View(iPNContext);
        }

        // POST: IPNContexts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var iPNContext = await _context.IPNContexts.FindAsync(id);
            _context.IPNContexts.Remove(iPNContext);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IPNContextExists(int id)
        {
            return _context.IPNContexts.Any(e => e.Id == id);
        }
    }
}
