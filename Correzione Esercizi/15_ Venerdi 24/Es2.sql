-- =====================================================
-- Esercizio: Query sulla tabella Vendite
-- Struttura:
-- Vendite(
--   id INT,
--   prodotto VARCHAR(100),
--   categoria VARCHAR(50),
--   quantita INT,
--   prezzo_unitario DECIMAL(6,2),
--   data_vendita DATE
-- )
-- =====================================================

-- 1) Totale vendite per categoria
SELECT categoria, COUNT(*) AS totale_vendite
FROM Vendite
GROUP BY categoria
ORDER BY totale_vendite DESC;

-- 2) Prezzo medio per categoria
SELECT categoria, ROUND(AVG(prezzo_unitario), 2) AS prezzo_medio
FROM Vendite
GROUP BY categoria
ORDER BY prezzo_medio DESC;

-- 3) Quantità totale venduta per ogni prodotto
SELECT prodotto, SUM(quantita) AS quantita_totale
FROM Vendite
GROUP BY prodotto
ORDER BY quantita_totale DESC;

-- 4) Prezzo massimo e minimo nella tabella
SELECT
  MAX(prezzo_unitario) AS prezzo_massimo,
  MIN(prezzo_unitario) AS prezzo_minimo
FROM Vendite;

-- 5) Prodotto con prezzo massimo
SELECT prodotto, prezzo_unitario
FROM Vendite
ORDER BY prezzo_unitario DESC
LIMIT 1;

-- 6) Prodotto con prezzo minimo
SELECT prodotto, prezzo_unitario
FROM Vendite
ORDER BY prezzo_unitario ASC
LIMIT 1;

-- 7) Numero totale di righe (vendite) nella tabella
SELECT COUNT(*) AS numero_righe
FROM Vendite;

-- 8) Totale delle quantità vendute (somma di tutte le quantità)
SELECT SUM(quantita) AS totale_quantita
FROM Vendite;

-- 9) I 5 prodotti più costosi (in base al prezzo_unitario)
SELECT prodotto, MAX(prezzo_unitario) AS prezzo_max
FROM Vendite
GROUP BY prodotto
ORDER BY prezzo_max DESC
LIMIT 5;

-- 10) I 3 prodotti meno venduti per quantità totale
SELECT prodotto, SUM(quantita) AS totale_quantita
FROM Vendite
GROUP BY prodotto
ORDER BY totale_quantita ASC
LIMIT 3;

-- 11) I 3 prodotti con la quantità totale più alta (più venduti)
SELECT prodotto, SUM(quantita) AS totale_quantita
FROM Vendite
GROUP BY prodotto
ORDER BY totale_quantita DESC
LIMIT 3;

-- =====================================================
-- FINE FILE
-- =====================================================
