using TDD.Exceptions;
using TDD.Models;
using TDD.Repositories.Interfaces;

namespace TDD.services;

public class BookWebService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookWebService _bookWebService;

    public BookWebService(IBookRepository bookRepository, IBookWebService bookWebService)
    {
        _bookRepository = bookRepository;
        _bookWebService = bookWebService;
    }

    public async Task<Book> CompleteBookWithWebService(string isbn)
    {
        Book? bookFind = await _bookWebService.FindBookByIsbn(isbn);
        if (bookFind == null)
        {
            throw new WebServiceDontFindBookByIsbn();
        }

        return await _bookRepository.Save(bookFind);
    }
}