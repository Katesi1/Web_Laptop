using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using AutoMapper;
using MvcLaptop.Services;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LaptopsController : Controller
    {
        private readonly MvcLaptopContext _context;
        private readonly ILaptopService _laptopService;
        public LaptopsController(MvcLaptopContext context, ILaptopService laptopService)
        {
            _context = context;
            _laptopService = laptopService;
        }

        // GET: Laptops
        public async Task<IActionResult> Index(string searchString)
        {
            // Lấy danh sách laptop từ database
            var laptops = await _laptopService.GetLaptops(searchString);

            // Lấy thông tin người dùng từ Session
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewData["SearchString"] = searchString;
            return View(laptops);
        }

        
        // GET: Laptops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            return View(laptop);
        }

        // GET: Laptops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Laptops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LaptopRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _laptopService.Create(request);
                if(result == null)
                    return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                // Lưu dữ liệu
                ViewBag.SuccessMessage = "Thêm sản phẩm thành công!";
                return View();
            }

            return View(request);
        }
        // GET: Laptops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            return View(laptop);
        }

        // POST: Laptops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,LaptopViewModel laptop)
        {
            if (id != laptop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _laptopService.Update(id, laptop);
                    if(result)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_laptopService.LaptopExists(laptop.Id))
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
            return View(laptop);
        }

        // GET: Laptops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }

            return View(laptop);
        }

        // POST: Laptops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _laptopService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
