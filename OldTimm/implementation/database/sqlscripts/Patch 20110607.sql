ALTER TABLE fivezipareas ADD HOME_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE fivezipareas ADD BUSINESS_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE fivezipareas ADD APT_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE fivezipareas ADD TOTAL_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE threezipareas ADD HOME_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE threezipareas ADD BUSINESS_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE threezipareas ADD APT_COUNT INT NOT NULL DEFAULT 0;
ALTER TABLE threezipareas ADD TOTAL_COUNT INT NOT NULL DEFAULT 0;

UPDATE fivezipareas
    INNER JOIN (
        SELECT ZIP,
            SUM(HOME_COUNT) AS HOME_COUNT,
            SUM(BUSINESS_COUNT) AS BUSINESS_COUNT,
            SUM(APT_COUNT) AS APT_COUNT,
            SUM(TOTAL_COUNT) AS TOTAL_COUNT
            FROM (
                SELECT GEOCODE,
                    ZIP,
                    HOME_COUNT,
                    BUSINESS_COUNT,
                    APT_COUNT,
                    TOTAL_COUNT
                    FROM premiumcroutes
                GROUP BY GEOCODE,
                    ZIP,
                    HOME_COUNT,
                    BUSINESS_COUNT,
                    APT_COUNT,
                    TOTAL_COUNT
            ) AS t
        GROUP BY ZIP
    ) AS p ON fivezipareas.Code = p.ZIP
    SET fivezipareas.HOME_COUNT = p.HOME_COUNT,
    fivezipareas.BUSINESS_COUNT = p.BUSINESS_COUNT,
    fivezipareas.APT_COUNT = p.APT_COUNT,
    fivezipareas.TOTAL_COUNT = p.TOTAL_COUNT
;
UPDATE threezipareas
    INNER JOIN (
        SELECT LEFT(ZIP, 3) AS ZIP,
            SUM(HOME_COUNT) AS HOME_COUNT,
            SUM(BUSINESS_COUNT) AS BUSINESS_COUNT,
            SUM(APT_COUNT) AS APT_COUNT,
            SUM(TOTAL_COUNT) AS TOTAL_COUNT
            FROM (
                SELECT GEOCODE,
                    ZIP,
                    HOME_COUNT,
                    BUSINESS_COUNT,
                    APT_COUNT,
                    TOTAL_COUNT
                    FROM premiumcroutes
                GROUP BY GEOCODE,
                    ZIP,
                    HOME_COUNT,
                    BUSINESS_COUNT,
                    APT_COUNT,
                    TOTAL_COUNT
            ) AS t
        GROUP BY LEFT(ZIP, 3)
    ) AS p ON threezipareas.Code = p.ZIP
    SET threezipareas.HOME_COUNT = p.HOME_COUNT,
    threezipareas.BUSINESS_COUNT = p.BUSINESS_COUNT,
    threezipareas.APT_COUNT = p.APT_COUNT,
    threezipareas.TOTAL_COUNT = p.TOTAL_COUNT
;