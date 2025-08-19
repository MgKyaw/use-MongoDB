using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDbTest.Models;
using MongoDbTest.Services;

namespace MongoDbTest.Controllers
{
  [Authorize]
  public class ExploreDbController : Controller
  {
    private readonly DocumentService _documentService;

    public ExploreDbController(DocumentService documentService)
    {
      _documentService = documentService;
    }

    public async Task<IActionResult> Index(string selectedDatabase, string selectedCollection, int index = 0)
    {
      var databasesAndCollections = await _documentService.GetDatabasesAndCollections();
      var viewModel = new ExplorerDbViewModel()
      {
        DatabasesAndCollections = databasesAndCollections,
        Database = selectedDatabase,
        Collection = selectedCollection,
        Index = index
      };
      if (selectedCollection != null && selectedDatabase != null)
      {
        viewModel.Document = await _documentService.GetDocument(selectedDatabase, selectedCollection, index);
        viewModel.CollectionCount = await _documentService.GetCollectionCount(selectedDatabase, selectedCollection);
      }
      return View(viewModel);
    }

    public async Task<IActionResult> CreateOrUpdate(
      string database,
      string collection,
      string id,
      int index,
      string fieldName,
      string value
    )
    {
      await _documentService.CreateOrUpdateField(database, collection, id, fieldName, value);
      return RedirectToAction("Index", GetRouteValues(database, collection, index));
    }

    public async Task<IActionResult> DeleteDoc(
      string database,
      string collection,
      string id,
      int index
    )
    {
      var delete = await _documentService.DeleteDocument(database, collection, id);
      return RedirectToAction("Index", GetRouteValues(database, collection, index));
    }

    public async Task<IActionResult> CreateDoc(
      string database,
      string collection
    )
    {
      await _documentService.CreateDocument(database, collection);
      var count = await _documentService.GetCollectionCount(database, collection);
      return RedirectToAction("Index", GetRouteValues(database, collection, count - 1));
    }

    private static object GetRouteValues(string database, string collection, long index)
    {
      return new { selectedDatabase = database, selectedCollection = collection, index = index };
    }
  }
}