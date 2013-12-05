
ALTER TABLE B$GC_CBL DISABLE ALL TRIGGERS;
/

DECLARE
       
    V_COUNT_ANN     VARCHAR2(1000);

    CURSOR CUR1 IS
        SELECT  A.G3E_FNO, A.G3E_FID
        FROM B$GC_CBL A
        WHERE EXISTS (SELECT 'A' FROM B$GC_COUNT B
        WHERE A.G3E_FNO = B.G3E_FNO
        AND A.G3E_FID = B.G3E_FID
        AND B.MDF_UNIT IS NOT NULL
        AND A.CABLE_CLASS = 'E-CABLE'
        );
                        
    BEGIN

        
        FOR REC1 IN CUR1
        LOOP
                
            V_COUNT_ANN := '';
                    
            DECLARE
            
                 V_COUNT NUMBER := 1;
         
                 CURSOR CUR2 IS
                    SELECT C.EXC_ABB, B.MDF_UNIT, B.VERTICAL_UNIT, B.LOW_PORT, B.HIGH_PORT, B.ELO_PR, B.EHI_PR
                    FROM B$GC_COUNT B, B$GC_NETELEM C
                    WHERE REC1.G3E_FNO = B.G3E_FNO
                    AND REC1.G3E_FID = B.G3E_FID
                    AND REC1.G3E_FNO = C.G3E_FNO
                    AND REC1.G3E_FID = C.G3E_FID
                    AND B.MDF_UNIT IS NOT NULL
                    ORDER BY B.G3E_CID;         
            
            BEGIN
                

                FOR REC2 IN CUR2
                LOOP
                 
    
                    IF V_COUNT > '1' THEN
                            V_COUNT_ANN := V_COUNT_ANN || CHR(10);
                    END IF;
                    
                    V_COUNT_ANN := V_COUNT_ANN || REC2.EXC_ABB ||'/'|| REC2.MDF_UNIT ||'/'|| REC2.VERTICAL_UNIT ||'/'|| REC2.LOW_PORT ||'-'|| REC2.HIGH_PORT ||'/'|| REC2.ELO_PR ||'-'|| REC2.EHI_PR ||'/'|| (REC2.EHI_PR-REC2.ELO_PR+1) || ' PSG';                
                                        
                    V_COUNT := V_COUNT + 1;
                            
                END LOOP;
                
                DBMS_OUTPUT.PUT_LINE('FNO=' || REC1.G3E_FNO ||' : FID='|| REC1.G3E_FID || 'COUNT ANNOTN=' || V_COUNT_ANN);
                
            END;
            
            UPDATE B$GC_CBL A SET A.COUNT_ANNOTATION = V_COUNT_ANN 
                WHERE A.G3E_FNO = REC1.G3E_FNO
                AND A.G3E_FID = REC1.G3E_FID;
        

            COMMIT;
            
        
        END LOOP;




    END;
/

ALTER TABLE B$GC_CBL ENABLE ALL TRIGGERS;
/
