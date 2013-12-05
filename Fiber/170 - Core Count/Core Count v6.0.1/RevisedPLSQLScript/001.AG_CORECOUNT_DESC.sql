CREATE OR REPLACE FUNCTION NEPS.AG_CORECOUNT_DESC(V_G3E_FID NUMBER, V_G3E_CID NUMBER DEFAULT 1)
RETURN VARCHAR2
AS
    V_TEMP              VARCHAR2(4000);

    V_FNO1              NUMBER;
    V_FID1              NUMBER;
    V_LOW1              VARCHAR2(40);
    V_HIGH1             VARCHAR2(40);

	V_FNO2              NUMBER;
    V_FID2              NUMBER;
    V_LOW2              VARCHAR2(40);
    V_HIGH2             VARCHAR2(40);
	
    V_SRC_FNO           VARCHAR2(40);
    V_SRC_FID           VARCHAR2(40);
    V_SRC_LOW           VARCHAR2(40);
    V_SRC_HIGH          VARCHAR2(40);
    V_EXC_ABB           VARCHAR2(40);
    V_FEATURE_STATE     VARCHAR2(40);
    V_CABLE_CODE        VARCHAR2(40);
    V_CORE_STATUS       VARCHAR2(40);
    V_G3E_FNO           NUMBER;

    V_SRC               VARCHAR2(40);

    V_LOWHIGH           VARCHAR2(40);
    V_LOWHIGH1          VARCHAR2(40);
	V_LOWHIGH2          VARCHAR2(40);

    V_LOW               VARCHAR2(40);
    V_HIGH              VARCHAR2(40);

    V_DEVICE_NAME       VARCHAR2(40);
    V_FDC_FID           VARCHAR2(40);
	V_FDC_CODE          VARCHAR2(40);
	V_FDP_CODE			VARCHAR2(40);


    V_COUNT             NUMBER;
    V_STS               VARCHAR2(40);
/*
	AUTHOR     : OLAF
	Date       : Oct 2012
	Description: display core count text from gc_splice and gc_corecount
	Version    : V3
 */
--mike 2012 10 02 ADDED: and CORE_STATUS is not null
    CURSOR X (V_G3E_FID NUMBER)
    IS
    SELECT TRIM(DECODE(TRIM(LOW2),'*',NULL,'***',NULL,TRIM(LOW2))) AS LOW2
        , TRIM(DECODE(TRIM(HIGH2),'*',NULL,'***',NULL,TRIM(HIGH2))) AS HIGH2
        , TRIM(DECODE(TRIM(SRC_LOW),'*',NULL,'***',NULL,TRIM(SRC_LOW))) AS SRC_LOW
        , TRIM(DECODE(TRIM(SRC_HIGH),'*',NULL,'***',NULL,TRIM(SRC_HIGH))) AS SRC_HIGH
        , TRIM(DECODE(TRIM(EXC_ABB),'*',NULL,'***',NULL,TRIM(EXC_ABB))) AS EXC_ABB
        , TRIM(DECODE(TRIM(FEATURE_STATE),'*',NULL,'***',NULL,TRIM(FEATURE_STATE))) AS FEATURE_STATE
        , TRIM(DECODE(TRIM(CABLE_CODE),'*',NULL,'***',NULL,TRIM(CABLE_CODE))) AS CABLE_CODE
        , TRIM(DECODE(TRIM(CORE_STATUS),'*',NULL,'***',NULL,TRIM(CORE_STATUS))) AS CORE_STATUS
        , G3E_FNO
		, FNO2, FID2, SRC_FID, SRC_FNO, FNO1, FID1, LOW1, HIGH1
    FROM GC_SPLICE_CONNECT
    WHERE G3E_FID = V_G3E_FID and CORE_STATUS is not null
    ORDER BY CORE_STATUS, SRC_LOW ASC,G3E_CID;

    CURSOR Y (V_G3E_FID NUMBER)
    IS
       SELECT C.G3E_FID,
                CORE_STATUS,
                TRIM(C.EXC_ABB) EXC_ABB,
                TRIM(C.CABLE_CODE) CABLE_CODE,
                C.LOW,
                C.HIGH,
                C.G3E_FNO
            FROM GC_CORECOUNT C
            WHERE G3E_FID = V_G3E_FID
            ORDER BY C.LOW,G3E_CID;


    XX X%ROWTYPE;
    YY Y%ROWTYPE;


BEGIN
    /*

these 3 that should have SPARE/STUMP symbol
WHEN 5100   THEN v_comp_t3 := 'GC_FDC_S' ;
WHEN 5600   THEN v_comp_t3 := 'GC_FDP_S' ;
WHEN 11800  THEN v_comp_t3 := 'GC_FSPLICE_S' ;

pls correct me if you see different from LNMs
*/

    SELECT COUNT(*)
    INTO V_COUNT
    FROM GC_CORECOUNT
    WHERE G3E_FID = V_G3E_FID;

    IF V_COUNT > 0 THEN
        dbms_output.put_line('---- GC_CORECOUNT ----');

        OPEN Y (V_G3E_FID);
        LOOP
            FETCH Y INTO YY;
            EXIT WHEN Y%NOTFOUND;
--
--            dbms_output.put_line(yy.core_status);
--            dbms_output.put_line(yy.CABLE_CODE);
--            dbms_output.put_line(yy.EXC_ABB);
--            dbms_output.put_line(yy.LOW);
--            dbms_output.put_line(yy.HIGH);
--            dbms_output.put_line(yy.G3E_FNO);
--
			V_CORE_STATUS := YY.CORE_STATUS;
			V_CABLE_CODE  := YY.CABLE_CODE;
			V_EXC_ABB     := YY.EXC_ABB;
			V_LOW         := YY.LOW;
			V_HIGH        := YY.HIGH;
			V_G3E_FNO     := YY.G3E_FNO;

dbms_output.put_line('--'||V_CORE_STATUS||'-'||V_FNO2||'-');
            IF V_CORE_STATUS = 'MAIN' THEN
                V_CORE_STATUS := 'M';
            ELSIF V_CORE_STATUS = 'PROTECTION' THEN
                V_CORE_STATUS := 'P';
			ELSIF V_CORE_STATUS = 'FOMS' THEN -- add by Catherine 07 jan 2013
                V_CORE_STATUS := 'F';           
            ELSIF V_CORE_STATUS = 'SPARE' THEN
				V_CORE_STATUS := 'SP';			
			ELSIF V_CORE_STATUS = 'STUMP' THEN
				V_CORE_STATUS := 'ST';
            END IF;


			IF V_STS = V_CORE_STATUS THEN
				--space for repeated V_CORE_STATUS
				V_TEMP := V_TEMP ||'   ' ;
			ELSE
				V_STS := V_CORE_STATUS;
				V_TEMP := V_TEMP || V_STS ||':' ;
			END IF ;

			IF V_LOW = V_HIGH THEN
				V_LOWHIGH := V_LOW;
			ELSIF V_LOW IS NOT NULL AND V_HIGH IS NOT NULL THEN
				V_LOWHIGH := V_LOW || '-' || V_HIGH;
			ELSE
				V_LOWHIGH := NULL;
			END IF;

			--LNMS no comma, take away
			--V_TEMP := V_TEMP || V_EXC_ABB||' '||V_CABLE_CODE||', '|| V_LOWHIGH || chr(10) ;
			V_TEMP := V_TEMP || V_EXC_ABB||' '||V_CABLE_CODE||' '|| V_LOWHIGH || chr(10) ;

        END LOOP;
        CLOSE Y;
    ELSE
        dbms_output.put_line('---- GC_SPLICE_CONNECT ----');
        OPEN X (V_G3E_FID);

        LOOP
			FETCH X INTO XX;
			EXIT WHEN X%NOTFOUND;

            V_LOW1          := XX.LOW1;
            V_HIGH1         := XX.HIGH1;
            V_LOW2          := XX.LOW2;
            V_HIGH2         := XX.HIGH2;
            V_SRC_LOW       := XX.SRC_LOW;
            V_SRC_HIGH      := XX.SRC_HIGH;
            V_EXC_ABB       := XX.EXC_ABB;
            V_FEATURE_STATE := XX.FEATURE_STATE;
            V_CABLE_CODE    := XX.CABLE_CODE;
            V_CORE_STATUS   := XX.CORE_STATUS;
            V_G3E_FNO       := XX.G3E_FNO;
			V_FNO1          := XX.FNO1; --mike added, assume splitter always FID2
			V_FID1          := XX.FID1; --mike added, assume splitter always FID2
			V_FNO2          := XX.FNO2; --mike added, assume splitter always FID2
			V_FID2          := XX.FID2; --mike added, assume splitter always FID2
			V_SRC_FID       := XX.SRC_FID; --mike added, get FDC splitter info for FDP
			V_FDC_CODE	:= '';

            /*
                IF SRCINATION DEVICE THEN LABEL FORMAT IS :

                [CORE_STATUS] : [CABLE_CODE] [LOWHIGH2] [SRCLOHI]

                IF SPLICE DEVICE THEN LABEL FORMAT IS :

                [CORE_STATUS] : [EXC_ABB] [CABLE_CODE] [G3E_TEXT_FROM_VINOD_PROGRAM]
            */

            IF V_CORE_STATUS = 'MAIN' THEN
                V_CORE_STATUS := 'M';
            ELSIF V_CORE_STATUS = 'PROTECTION' THEN
                V_CORE_STATUS := 'P';
            ELSIF V_CORE_STATUS = 'FOMS' THEN -- add by Catherine 07 jan 2013
                V_CORE_STATUS := 'F';           
            ELSIF V_CORE_STATUS = 'SPARE' THEN
				V_CORE_STATUS := 'SP';			
			ELSIF V_CORE_STATUS = 'STUMP' THEN
				V_CORE_STATUS := 'ST';
            END IF;

            IF V_SRC_LOW = V_SRC_HIGH THEN
                V_SRC := V_SRC_LOW;
            ELSIF V_SRC_LOW IS NOT NULL AND V_SRC_HIGH IS NOT NULL THEN
                V_SRC := V_SRC_LOW || '-' || V_SRC_HIGH;
            ELSE
                V_SRC := NULL;
            END IF;


            IF V_LOW1 = V_HIGH1 THEN
                V_LOWHIGH1 := V_LOW1;
            ELSIF V_LOW1 IS NOT NULL AND V_HIGH1 IS NOT NULL THEN
                V_LOWHIGH1 := V_LOW1 || '-' || V_HIGH1;
            ELSE
                V_LOWHIGH1 := NULL;
            END IF;
            

            IF V_LOW2 = V_HIGH2 THEN
                V_LOWHIGH2 := V_LOW2;
            ELSIF V_LOW2 IS NOT NULL AND V_HIGH2 IS NOT NULL THEN
                V_LOWHIGH2 := V_LOW2 || '-' || V_HIGH2;
            ELSE
                V_LOWHIGH2 := NULL;
            END IF;

            IF V_STS = V_CORE_STATUS THEN
                 V_TEMP := V_TEMP ||'  ';
            ELSE
                V_STS := V_CORE_STATUS;
                V_TEMP := V_TEMP || V_STS ||' : ';
            END IF;

           --IF ((V_G3E_FNO IN (10800,11800)) AND (V_FNO1 in (7200))) THEN -- only for E-Side
		   IF (V_G3E_FNO IN (10800,11800)) THEN 
				IF V_EXC_ABB IS NOT NULL THEN
					V_TEMP := TRIM(V_TEMP || V_EXC_ABB);
				END IF;
            END IF;

            IF V_CABLE_CODE IS NOT NULL THEN
                V_TEMP := V_TEMP || ' ' || V_CABLE_CODE ;
            END IF;

			
			-- IF SPLICE for DSIDE - Add additional line [splitter #] [Splitter code] [spliter port range]
			IF ((V_G3E_FNO IN (11800)) AND (V_FNO1 IN (7400))) THEN
			
				V_TEMP := V_TEMP || ' ' || V_LOWHIGH1 ;
				dbms_output.put_line('---- ADD NEW LINE FOR SOURCE SPLITTER ----');
				
				select SPLITTER_CODE into V_DEVICE_NAME from GC_FSPLITTER where G3E_FID=V_SRC_FID ;
				-- Get FDC code
				BEGIN
					SELECT FDC_CODE INTO V_FDC_CODE FROM GC_FDC 
					WHERE G3E_FID = (SELECT G3E_FID FROM GC_OWNERSHIP WHERE G3E_ID = (SELECT OWNER1_ID FROM GC_OWNERSHIP WHERE G3E_FID = V_SRC_FID));
				END;
				IF V_SRC_LOW = V_SRC_HIGH THEN
					V_LOWHIGH := V_SRC_LOW;
				ELSIF V_SRC_LOW IS NOT NULL AND V_SRC_HIGH IS NOT NULL THEN
					V_LOWHIGH := V_SRC_LOW || '-' || V_SRC_HIGH;
				ELSE
					V_LOWHIGH := NULL;
				END IF;
				-- Add FDP CODE before Splitter 
				select SPLITTER_CODE into V_DEVICE_NAME from GC_FSPLITTER where G3E_FID=V_SRC_FID;
				V_TEMP := TRIM(V_TEMP) || CHR(10) || '     ' || V_FDC_CODE || ' ' || V_DEVICE_NAME || ' ' || V_LOWHIGH||'';
				V_SRC := NULL;
			END IF;
			
			
			-- IF GC_FDP = 5600			
			-- IF GC_FDP, mike 2012 10 18 (, 5900, 9900)
			IF V_G3E_FNO IN (5600) THEN
				-- Format 2 line : 
				-- line 1: [M or FOMS]: [CABLE_CODE] [CORE] [FDP SPLITTER_CODE] [FDP_SPLITTER_CODE]
				-- line 2: [FDC Code][FDC SPLITTER CODE] [FDC SPLITTER PORT]
				dbms_output.put_line('---- GC_FDP ----');
				--select FDP_TYPE into V_DEVICE_NAME from GC_FDP where G3E_FID=V_G3E_FID ;
				--IF V_DEVICE_NAME='FOMS FDP' THEN
				--	V_TEMP := 'FOMS : ';
				--ELSE
				--	V_TEMP := 'M : ';
				--END IF;
				
					
					select SPLITTER_CODE into V_DEVICE_NAME from GC_FSPLITTER where G3E_FID=V_FID2 ;
					--V_TEMP := TRIM(V_TEMP) || ' ' || V_CABLE_CODE || ' ' || V_LOWHIGH1 || '  ' || V_DEVICE_NAME || ' ' || V_LOWHIGH2 ;
					V_TEMP := TRIM(V_TEMP) || ' ' || V_LOWHIGH1 || '  ' || V_DEVICE_NAME || ' ' || V_LOWHIGH2 ;
					-- Get FDC code
					BEGIN
						SELECT FDC_CODE INTO V_FDC_CODE FROM GC_FDC 
						WHERE G3E_FID = (SELECT G3E_FID FROM GC_OWNERSHIP WHERE G3E_ID = (SELECT OWNER1_ID FROM GC_OWNERSHIP WHERE G3E_FID = V_SRC_FID));
					END;
					--CASE V_G3E_FNO
					--	WHEN 5600 THEN select FDC_CODE, FDC_FID, FDP_CODE into V_DEVICE_NAME, V_FDC_FID, V_FDP_CODE from GC_FDP where G3E_FID=V_FID2 ;
					--	WHEN 5900 THEN select FDC_CODE, FDC_FID, FDP_CODE into V_DEVICE_NAME, V_FDC_FID, V_FDP_CODE from GC_FTB where G3E_FID=V_FID2 ;
					--	WHEN 9900 THEN select FDC_CODE, FDC_FID, DB_CODE into V_DEVICE_NAME, V_FDC_FID, V_FDP_CODE from GC_DB where G3E_FID=V_FID2 ;
					--END CASE;
					--V_TEMP := TRIM(V_TEMP) || V_DEVICE_NAME;
					-- GET SRC LOW HIGH
					IF V_SRC_LOW = V_SRC_HIGH THEN
						V_LOWHIGH := V_SRC_LOW;
					ELSIF V_SRC_LOW IS NOT NULL AND V_SRC_HIGH IS NOT NULL THEN
						V_LOWHIGH := V_SRC_LOW || '-' || V_SRC_HIGH;
					ELSE
						V_LOWHIGH := NULL;
					END IF;
					-- Add FDP CODE before Splitter 
					select SPLITTER_CODE into V_DEVICE_NAME from GC_FSPLITTER where G3E_FID=V_SRC_FID;
					V_TEMP := TRIM(V_TEMP) || CHR(10) || '     ' || V_FDC_CODE || ' ' || V_DEVICE_NAME || ' ' || V_LOWHIGH;
					V_SRC := NULL;
			END IF;
			--END IF FDP
			
            -- IF GC_FDC
            IF V_G3E_FNO = 5100 THEN

                
                IF (V_FNO2 = 12300) AND ( V_CORE_STATUS = 'M'  or  V_CORE_STATUS = 'P' or  V_CORE_STATUS = 'FOMS M' or  V_CORE_STATUS = 'FOMS P' ) THEN
					IF V_SRC_LOW IS NOT NULL THEN
						V_TEMP := TRIM(V_TEMP || ' ' || V_SRC_LOW);
					END IF;

					BEGIN
                        SELECT SPLITTER_CODE
                        INTO V_DEVICE_NAME
                        FROM GC_FSPLITTER
                        WHERE G3E_FID = V_FID2
                        AND ROWNUM < 2;
						V_TEMP := TRIM(V_TEMP || ' ' || V_DEVICE_NAME);
                    EXCEPTION
                        WHEN OTHERS THEN
                            V_DEVICE_NAME := 'FSPLITTER_NOT_FOUND';
                    END;
					
					IF V_LOW2 IS NOT NULL THEN
						V_TEMP := TRIM(V_TEMP || ' ' || V_LOW2 );
					END IF;
				 
                ELSE

					 IF V_LOWHIGH2 IS NOT NULL THEN
						V_TEMP := TRIM(V_TEMP || ' ' || V_LOWHIGH2 );
					 END IF;
				END IF;
            -- IF NOT GC_FDC
			ELSE
				IF V_SRC IS NOT NULL THEN
                    V_TEMP := TRIM(V_TEMP || ' ' || V_SRC);
                END IF;
				
				
            END IF;

            V_TEMP := TRIM(V_TEMP) || CHR(10);

        END LOOP;
        CLOSE X;

    END IF;
    --V_TEMP := TRIM(REGEXP_REPLACE(V_TEMP, '\s{2,}',''));
    RETURN V_TEMP;
EXCEPTION
    WHEN OTHERS
        THEN
          DBMS_OUTPUT.PUT_LINE(DBMS_UTILITY.FORMAT_ERROR_BACKTRACE() );
          DBMS_OUTPUT.PUT_LINE(DBMS_UTILITY.FORMAT_ERROR_STACK);
            IF Y%ISOPEN
            THEN
              CLOSE Y;
            END IF;
            IF X%ISOPEN
            THEN
              CLOSE X;
            END IF;

            RETURN NULL;
END;
/

--exec LTT_USER.VIEWJOB('SBJ-FIBER (EQUIP)-38-2012');
--splice with spare and stump
--select AG_CORECOUNT_DESC(106201921) from dual;
--FDC
--select AG_CORECOUNT_DESC(106201937) from dual;
--FDP
--select AG_CORECOUNT_DESC(106201963) from dual;
-- DSIDE SPLICE
select AG_CORECOUNT_DESC(106201947) from dual;