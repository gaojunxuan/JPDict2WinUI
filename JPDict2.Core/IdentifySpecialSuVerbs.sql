CREATE VIEW SpecialSuVerbs
AS
    SELECT Definition.* FROM Definition
    INNER JOIN
    (SELECT Id FROM Sense WHERE Pos LIKE "%su verb - precursor to the modern suru%" AND Pos LIKE "%Godan verb with 'su' ending%") A2
    ON (Definition.Senses LIKE '%,' || A2.Id || ',%' OR
        Definition.Senses LIKE '%,' || A2.Id OR
        Definition.Senses LIKE '%,' || A2.Id OR
        Definition.Senses = A2.Id
    );

SELECT * FROM Keyword WHERE Keyword.DefEntryId IN (SELECT Id FROM SpecialSuVerbs);