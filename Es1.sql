-- 1) Regioni dei Paesi in Europa (senza ripetizioni)
SELECT DISTINCT Region
FROM Country
WHERE Continent = 'Europe'
ORDER BY Region;


-- 2) Città USA con popolazione > 1.000.000, dalla più popolosa alla meno
SELECT Name, Population
FROM City
WHERE CountryCode = 'USA'
  AND Population > 1000000
ORDER BY Population DESC;


-- 3) Per ogni continente: numero di paesi e popolazione totale
SELECT
  Continent,
  COUNT(*)              AS num_countries,
  SUM(Population)       AS total_population
FROM Country
GROUP BY Continent
ORDER BY total_population DESC;
