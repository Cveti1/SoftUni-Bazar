using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models.Ad;
using SoftUniBazar.Models.Category;

namespace SoftUniBazar.Controllers
{
    [Authorize]
    public class AdController : Controller
    {

        private readonly BazarDbContext data;

        public AdController(BazarDbContext _data)
        {
            data = _data;
        }

       public async Task<IActionResult> All()
        {
            var allAds = await data
                .Ads
                .Select(a => new AdViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                    Category = a.Category.Name,
                    Owner = a.Owner.UserName
                })
                .ToListAsync();

            return View(allAds);
        }


        [HttpGet]
       public async Task<IActionResult> Add()
       {
           AdFormModel adModel = new AdFormModel()
           {
               Categories = GetCategory()
           };

           return View(adModel);
       }

       [HttpPost]
       public async Task<IActionResult> Add(AdFormModel adModel)
       {
           if (!GetCategory().Any(e => e.Id == adModel.CategoryId))
           {
               ModelState.AddModelError(nameof(adModel.CategoryId), "Category does not exist!");
           }

           if (!ModelState.IsValid)
           {
               return View(adModel);
           }

           string currentUserId = GetUserId();

           var newAd = new Ad()
           {
               Name = adModel.Name,
               Description = adModel.Description,
               CreatedOn = DateTime.Now,
               ImageUrl = adModel.ImageUrl,
               Price= adModel.Price,
               CategoryId = adModel.CategoryId,
               OwnerId = currentUserId
           };

           await data.Ads.AddAsync(newAd);
           await data.SaveChangesAsync();

           return RedirectToAction("All", "Ad");
       }


       public async Task<IActionResult> AddToCart(int id)
       {
           var adToAdd = await data
               .Ads
               .FindAsync(id);

           if (adToAdd == null)
           {
               return BadRequest();
           }

           string currentUserId = GetUserId();

           var entry = new AdBuyer()
           {
              AdId = adToAdd.Id,
              BuyerId = currentUserId,
           };

           if (await data.AdBuyers.ContainsAsync(entry))
           {
               return RedirectToAction("Cart", "Ad");
           }

           await data.AdBuyers.AddAsync(entry);
           await data.SaveChangesAsync();

           return RedirectToAction("Cart", "Ad");
       }

       public async Task<IActionResult> Cart()
       {
           string currentUserId = GetUserId();

           var userAds = await data
               .AdBuyers
               .Where(a => a.BuyerId == currentUserId)
               .Select(a => new AdViewModel()
               {
                   Id = a.Ad.Id,
                   Name = a.Ad.Name,
                   Description = a.Ad.Description,
                   Price = a.Ad.Price,
                   ImageUrl = a.Ad.ImageUrl,
                   CreatedOn = a.Ad.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                   Category = a.Ad.Category.Name,
                   Owner = a.Ad.Owner.ToString()
               })
               .ToListAsync();

            
            return View(userAds);
       }

       public async Task<IActionResult> RemoveFromCart(int id)
       {
           var adId = id;
           var currentUser = GetUserId();

           var adToRemove = data.Ads.FindAsync(adId);

           if (adToRemove == null)
           {
               return BadRequest();
           }

           var entry = await data.AdBuyers.FirstOrDefaultAsync(a => a.BuyerId == currentUser && a.AdId == adId);
           data.AdBuyers.Remove(entry);
           await data.SaveChangesAsync();

           return RedirectToAction("All", "Ad");
       }

        [HttpGet]
       public async Task<IActionResult> Edit(int id)
       {
           var adToEdit = await data.Ads.FindAsync(id);

           if (adToEdit == null)
           {
               return BadRequest();
           }

           string currentUserId = GetUserId();
           if (currentUserId != adToEdit.OwnerId)
           {
               return Unauthorized();
           }

           AdFormModel newModel = new AdFormModel()
           {
               Name = adToEdit.Name,
               Description = adToEdit.Description,
               ImageUrl = adToEdit.ImageUrl,
               Price = adToEdit.Price,
               CategoryId = adToEdit.CategoryId,
               Categories = GetCategory()
           };

           return View(newModel);
       }

       [HttpPost]
       public async Task<IActionResult> Edit(int id, AdFormModel newModel)
       {
           var adToEdit = await data.Ads.FindAsync(id);

           if (adToEdit == null)
           {
               return BadRequest();
           }

           string currentUser = GetUserId();
           if (currentUser != adToEdit.OwnerId)
           {
               return Unauthorized();
           }

           if (!GetCategory().Any(e => e.Id == newModel.CategoryId))
           {
               ModelState.AddModelError(nameof(newModel.CategoryId), "Category does not exist!");
           }

          
           adToEdit.Name = newModel.Name;
           adToEdit.Description = newModel.Description;
           adToEdit.ImageUrl = newModel.ImageUrl;
           adToEdit.Price = newModel.Price;
           adToEdit.CategoryId = newModel.CategoryId;
           

           await data.SaveChangesAsync();
           return RedirectToAction("All", "Ad");
       }


        private IEnumerable<CategoryViewModel> GetCategory()
           => data
               .Categories
               .Select(t => new CategoryViewModel()
               {
                   Id = t.Id,
                   Name = t.Name
               });

       private string GetUserId()
           => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
