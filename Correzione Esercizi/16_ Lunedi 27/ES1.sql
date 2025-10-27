-- =========================================================
-- Esercizio - Query sulla tabella Clienti
-- =========================================================

-- 1) Clienti con email su dominio Gmail
SELECT *
FROM Clienti
WHERE LOWER(email) LIKE '%@gmail.com';

-- (equivalente della 1: "email termina con @gmail.com")
SELECT *
FROM Clienti
WHERE LOWER(email) LIKE '%@gmail.com';

-- 2) Clienti con nome che inizia con la lettera 'A'
SELECT *
FROM Clienti
WHERE LOWER(nome) LIKE 'a%';

-- (equivalente della 2: "nome comincia con la lettera A")
SELECT *
FROM Clienti
WHERE LOWER(nome) LIKE 'a%';

-- 3) Clienti con cognome di esattamente 5 lettere
SELECT *
FROM Clienti
WHERE CHAR_LENGTH(TRIM(cognome)) = 5;

-- (equivalente della 3: stessa richiesta formulata diversamente)
SELECT *
FROM Clienti
WHERE CHAR_LENGTH(TRIM(cognome)) = 5;

-- 4) Clienti con età compresa tra 30 e 40 anni (inclusi)
SELECT *
FROM Clienti
WHERE eta BETWEEN 30 AND 40;

-- (equivalente della 4: stessa richiesta con inclusione estremi)
SELECT *
FROM Clienti
WHERE eta BETWEEN 30 AND 40;

-- 5) Clienti che vivono in città il cui nome contiene 'roma' (case-insensitive)
SELECT *
FROM Clienti
WHERE LOWER(citta) LIKE '%roma%';

-- (equivalente della 5: stessa richiesta formulata diversamente)
SELECT *
FROM Clienti
WHERE LOWER(citta) LIKE '%roma%';
