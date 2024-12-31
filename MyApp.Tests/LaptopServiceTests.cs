using Xunit;
using MvcLaptop.Models;
using MvcLaptop.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcLaptop.Data;
using System.Linq;
using AutoMapper;
using System.Threading;

public class LaptopServiceTests
{
    private readonly MvcLaptopContext _context;
    private readonly LaptopService _service;
    private readonly IMapper _mapper;

    public LaptopServiceTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<MvcLaptopContext>()
                        .UseInMemoryDatabase(databaseName: "TestDb")
                        .Options;
        _context = new MvcLaptopContext(options);

        // Mock IMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<LaptopRequest, Laptop>();
            cfg.CreateMap<Laptop, LaptopViewModel>();
            cfg.CreateMap<LaptopViewModel, Laptop>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Initialize LaptopService with the real context
        _service = new LaptopService(_context, _mapper);
    }

    // Ensure that the database is cleared and recreated before each test
    private void SetupDatabase()
    {
        _context.Database.EnsureDeleted();  // Ensure the database is deleted
        _context.Database.EnsureCreated();  // Recreate the database
    }

    [Fact]
    public async Task Create_ShouldAddLaptopToDatabase()
    {
        // Arrange
        SetupDatabase();  // Clear and recreate the database for each test
        var request = new LaptopRequest { Title = "Laptop AA",Genre = "AA",Description = "",Quantity = 10 , Price = 1000,ImageUrl ="https://anphat.com.vn/media/product/47499_laptop_lenovo_loq_15iax9_83gs001svn_anphatcomputer_1.jpg" };

        // Act
        var result = await _service.Create(request);
        await _context.SaveChangesAsync(); // Ensure changes are saved before querying

        // Assert
        var laptopInDb = _context.Laptop.FirstOrDefault();
        Assert.NotNull(laptopInDb);
        Assert.Equal("Laptop AA", laptopInDb.Title);
        Assert.Equal(1000, laptopInDb.Price);
    }

    [Fact]
    public async Task Delete_ShouldRemoveLaptopFromDatabase_WhenFound()
    {
        // Arrange
        SetupDatabase();  // Clear and recreate the database for each test
        var laptop = new Laptop { Id = 1, Title = "Laptop AA",Genre = "AA",Description = "",Quantity = 10 , Price = 1000,ImageUrl ="https://anphat.com.vn/media/product/47499_laptop_lenovo_loq_15iax9_83gs001svn_anphatcomputer_1.jpg" };
        _context.Laptop.Add(laptop);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.Delete(laptop.Id);
        await _context.SaveChangesAsync(); // Ensure changes are saved before querying

        // Assert
        Assert.True(result);
        var deletedLaptop = await _context.Laptop.FindAsync(laptop.Id);
        Assert.Null(deletedLaptop);
    }

    [Fact]
    public async Task GetLaptopById_ShouldReturnLaptop_WhenFound()
    {
        // Arrange
        SetupDatabase();  // Clear and recreate the database for each test
        var laptop = new Laptop { Id = 1, Title = "Laptop AA",Genre = "AA",Description = "",Quantity = 10 , Price = 1000,ImageUrl ="https://anphat.com.vn/media/product/47499_laptop_lenovo_loq_15iax9_83gs001svn_anphatcomputer_1.jpg" };
        _context.Laptop.Add(laptop);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLaptopById(laptop.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop AA", result.Title);
    }

    [Fact]
    public async Task GetLaptops_ShouldReturnListOfLaptops_WhenSearchStringIsNull()
    {
        // Arrange
        SetupDatabase();  // Clear and recreate the database for each test
        var laptops = new List<Laptop>
        {
            new Laptop { Id = 1, Title = "Laptop AA",Genre = "AA",Description = "",Quantity = 10 , Price = 1000,ImageUrl ="https://anphat.com.vn/media/product/47499_laptop_lenovo_loq_15iax9_83gs001svn_anphatcomputer_1.jpg" },
            new Laptop { Id = 2, Title = "Laptop B", Price = 1500 }
        };
        _context.Laptop.AddRange(laptops);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLaptops(null); // Ensure to pass null explicitly

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetLaptops_ShouldFilterResults_WhenSearchStringIsProvided()
    {
        // Ensure the database is cleared before each test
        SetupDatabase();

        // Arrange
        var laptops = new List<Laptop>
        {
            new Laptop { Id = 1, Title = "Laptop AA",Genre = "AA",Description = "",Quantity = 10 , Price = 1000,ImageUrl ="https://anphat.com.vn/media/product/47499_laptop_lenovo_loq_15iax9_83gs001svn_anphatcomputer_1.jpg" },
            new Laptop { Id = 2, Title = "Laptop B", Price = 1500 }
        };
        _context.Laptop.AddRange(laptops);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLaptops("AA");

        // Assert
        Assert.Single(result); // Only "Laptop A" should be returned
    }
}
