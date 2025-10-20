// Servizi indipendenti che espongono contratti (interfacce) e DTO.
// In pratica sarebbero processi separati con protocolli standard (SOAP/HTTP).
using System;
using System.Collections.Generic;

record ProdottoDto(string Nome);

interface ICatalogoService
{
    IEnumerable<ProdottoDto> GetCatalogo();
}

interface IOrdiniService
{
    void CreaOrdine(IEnumerable<ProdottoDto> prodotti);
}

class CatalogoService : ICatalogoService
{
    public IEnumerable<ProdottoDto> GetCatalogo() => new[] { new ProdottoDto("Pane"), new ProdottoDto("Latte") };
}

class OrdiniService : IOrdiniService
{
    public void CreaOrdine(IEnumerable<ProdottoDto> prodotti) =>
        Console.WriteLine($"Ordine creato con: {string.Join(", ", System.Linq.Enumerable.Select(prodotti, p => p.Nome))}");
}

class Program
{
    static void Main()
    {
        ICatalogoService catalogo = new CatalogoService();   // servizio 1
        IOrdiniService ordini = new OrdiniService();         // servizio 2
        var items = catalogo.GetCatalogo();
        ordini.CreaOrdine(items);
    }
}
