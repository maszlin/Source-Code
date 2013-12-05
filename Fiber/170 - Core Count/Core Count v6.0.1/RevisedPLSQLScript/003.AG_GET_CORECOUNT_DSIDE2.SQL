CREATE OR REPLACE FUNCTION NEPS.AG_GET_CORECOUNT_DSIDE2(
v_fFNO NUMBER, v_fFID NUMBER , v_fiLow number, v_fiHigh number,
 v_tFNO NUMBER, v_tFID NUMBER, v_tiLow number, v_tiHigh number, V_DEBUG integer default 0)

RETURN AG_CORECOUNT_TAB2 PIPELINED
as

	TYPE recNRConnect IS RECORD(
		g3e_fno    number(5),
		g3e_fid    number(10),
		in_FNO    number(5),
		in_FID   number(10),
		out_FNO    number(5),
		out_FID    number(10),
		eff_core	number(5)
	);
	tabNRConn     recNRConnect ;

	TYPE recB IS RECORD(
		FNO1    number(5),
		FID1    number(10),
		LOW1    number(5),
		HIGH1   number(5),
		FNO2    number(5),
		FID2    number(10),
		LOW2    number(5),
		HIGH2   number(5),
		Term_FNO    number(5),
		term_fid    number(10),
		term_low    number(5),
		term_high   number(5)
	);
	tabB          recB;

	type result_type is table of recB INDEX BY BINARY_INTEGER;
	result_arr result_type;

	cnt 	integer;
	mapping_arr_cnt	integer;
	cntResult integer;
	iFound integer;

	TYPE mapping_rec IS RECORD (
	    curr_core_id 	number(5),
		term_fno 	number(5),
		term_fid 	number(10),
		term_core_id	NUMBER(5),
		checked number(1)
	);

	OUT_OF_RANGE EXCEPTION;
	TYPE mapping_tab IS TABLE OF mapping_rec INDEX BY BINARY_INTEGER;
	mapping_arr        mapping_tab;

	c2          SYS_REFCURSOR;
	c3          SYS_REFCURSOR;
	v_sql         varchar(300);
	v_sql2         varchar(300);
	in_core number;
	cursor c1 is
		select fno1,fid1,low1, high1, fno2, fid2, low2, high2, term_fno, term_fid, term_low, term_high
		from gc_splice_connect where term_fno in (12300) and fid2 = v_fFID order by low2;
BEGIN
/*
	AUTHOR     : Catherine Lee
	Date       : Oct 2012
	Description: support core count module
	Version    : V3
 */
 mapping_arr_cnt := 0;
	IF ((v_fFNO = 12300) AND (v_tFNO <> 12300))  or ((v_fFNO <> 12300) AND (v_tFNO = 12300)) or ((v_fFNO = 7400) AND (v_tFNO = 7400)) THEN
		-- get from the from feature then see what is the allocated source.
		OPEN c1; -- open the cursor before fetching
		cnt := 1;
		mapping_arr_cnt := 0;
		LOOP
			FETCH c1 INTO tabB; -- fetches columns into variables
			EXIT WHEN c1%NOTFOUND;
			DBMS_OUTPUT.PUT_LINE('From  ' || tabB.FNO1 ||' ('||tabB.FID1 ||') - '||tabB.low1 ||'.. '||tabB.high1 );
			-- Dynamic Create Array

			--IF (tabB.FNO1 = 5500) OR (tabB.term_fno = 5500) THEN
				FOR i IN tabB.low2 .. tabB.high2
				LOOP
					mapping_arr(cnt).curr_core_id := i;
					mapping_arr(cnt).term_fno := tabB.term_fno;
					mapping_arr(cnt).term_fid := tabB.term_fid;
					mapping_arr(cnt).term_core_id := tabB.term_low + (i- tabB.low2);
					mapping_arr(cnt).checked := 0; -- initialize

					DBMS_OUTPUT.PUT_LINE(cnt||'      curr core ( ' || mapping_arr(cnt).curr_core_id ||' ) --> '|| mapping_arr(cnt).term_fno ||'('||mapping_arr(cnt).term_fid ||'- '||AG_GET_FCABLE_CODE(mapping_arr(cnt).term_fno ,mapping_arr(cnt).term_fid)||') :'||mapping_arr(cnt).term_core_id );
					cnt := cnt + 1;
					mapping_arr_cnt := mapping_arr_cnt + 1;
				END LOOP;


			--End IF;

			--cnt := cnt + 1;
		END LOOP;
		CLOSE c1; -- close the cursor after fetching
	END IF;
	IF (mapping_arr_cnt = 0) and (v_fFNO = 12300) THEN
		DBMS_OUTPUT.PUT_LINE('********** Check if source splitter from FDC ***************');
		v_sql := 'select substr(splitter_type, instr(splitter_type, '':'')+1) from gc_fsplitter where g3e_fid ='|| v_fFID;
		DBMS_OUTPUT.PUT_LINE(v_sql);

		-- fetch and create the matching spur no.
		cnt := 1;
		OPEN c2 FOR v_sql;
		LOOP
			FETCH c2 INTO in_core; -- fetches columns into variables
			EXIT WHEN c2%NOTFOUND;


				-- Now for the AssignedCore
				FOR ind in 1 .. in_core
				LOOP
					mapping_arr(ind).term_fno := v_fFNO;
					mapping_arr(ind).term_fid := v_fFID;
					mapping_arr(ind).term_core_id := ind;
					mapping_arr(ind).curr_core_id := ind;
					mapping_arr(ind).checked := 0; -- initialize
					mapping_arr_cnt := mapping_arr_cnt + 1;
				END LOOP;

		END LOOP;
		CLOSE c2;
	END IF;


	DBMS_OUTPUT.PUT_LINE(' Now doing Checking');
	-- Now Check selected Range
	FOR i IN 1 .. mapping_arr.count
	LOOP
		IF (mapping_arr(i).curr_core_id >= v_fiLow) and (mapping_arr(i).curr_core_id <= v_fiHigh) THEN
			mapping_arr(i).checked := 1;

			DBMS_OUTPUT.PUT_LINE(' Checking '|| i);
		END IF;
	END LOOP;

	-- Finally form the result List
	cntResult := 1;
	iFound := 0;
	DBMS_OUTPUT.PUT_LINE (' forming result ' || mapping_arr.first ||'..'|| mapping_arr.last ||' total count'|| mapping_arr.count);
	FOR i in 1 .. mapping_arr_cnt
	LOOP
		DBMS_OUTPUT.PUT_LINE (' loop at  '|| i || ' Check status ' ||mapping_arr(i).checked);
		IF (mapping_arr(i).checked = 1) THEN
			iFound := iFound + 1;
		    IF (iFound = 1) THEN -- Always initialize
			   result_arr(cntResult).FNO1 := v_fFNO;
			   result_arr(cntResult).FID1 := v_fFID;
			   result_arr(cntResult).LOW1 := v_fiLow;
			   result_arr(cntResult).HIGH1 := v_fiLow;
			   result_arr(cntResult).FNO2 := v_tFNO;
			   result_arr(cntResult).FID2 := v_tFID;
			   result_arr(cntResult).LOW2 := v_tiLow;
			   result_arr(cntResult).HIGH2 := v_tiLow;
			   result_arr(cntResult).term_FNO := mapping_arr(i).term_fno;
			   result_arr(cntResult).Term_FID := mapping_arr(i).term_fid;
			   result_arr(cntResult).term_LOW := mapping_arr(i).term_core_id;
			   result_arr(cntResult).term_high := mapping_arr(i).term_core_id;
			ELSE
			   IF (result_arr(cntResult).Term_FID = mapping_arr(i).term_fid) and ( (result_arr(cntResult).term_high + 1 ) = mapping_arr(i).term_core_id) THEN
					result_arr(cntResult).HIGH1 := result_arr(cntResult).HIGH1 + 1;
					result_arr(cntResult).HIGH2 := result_arr(cntResult).HIGH2 + 1;
					result_arr(cntResult).term_high := mapping_arr(i).term_core_id;
			   ELSE
					cntResult := cntResult + 1;
					result_arr(cntResult).FNO1 := v_fFNO;
					result_arr(cntResult).FID1 := v_fFID;
					result_arr(cntResult).LOW1 := result_arr(cntResult-1).HIGH1;
					result_arr(cntResult).HIGH1 := result_arr(cntResult-1).HIGH1;
					result_arr(cntResult).FNO2 := v_tFNO;
					result_arr(cntResult).FID2 := v_tFID;
					result_arr(cntResult).LOW2 := result_arr(cntResult-1).HIGH2;
					result_arr(cntResult).HIGH2 := result_arr(cntResult-1).HIGH2;
					result_arr(cntResult).term_FNO := mapping_arr(i).term_fno;
					result_arr(cntResult).Term_FID := mapping_arr(i).term_fid;
					result_arr(cntResult).term_LOW := mapping_arr(i).term_core_id;
					result_arr(cntResult).term_high := mapping_arr(i).term_core_id;

			   END IF;
			END IF;
			DBMS_OUTPUT.PUT_LINE('  *** selected : ' || i || ' term :- ' || mapping_arr(i).term_fno ||'('||mapping_arr(i).term_fid ||') :'||mapping_arr(i).term_core_id );

		END IF;
	END LOOP;
	-- Display results
	DBMS_OUTPUT.PUT_LINE('************* RESULT ********** ');

	-- if not Empty
	IF  (iFound > 0) THEN
		FOR i in 1 .. result_arr.Count
		LOOP
			DBMS_OUTPUT.PUT_LINE(result_arr(i).FNO1 ||',' || result_arr(i).FID1 ||',' ||result_arr(i).LOW1 ||',' || result_arr(i).HIGH1||','||result_arr(i).FNO2 ||',' || result_arr(i).FID2 ||',' ||result_arr(i).LOW2 ||',' || result_arr(i).HIGH2||','||result_arr(i).term_FNO ||',' || result_arr(i).term_FID ||',' ||result_arr(i).term_LOW ||',' || result_arr(i).term_HIGH);
			PIPE ROW(AG_CORECOUNT_ROW2( result_arr(i).FNO1, result_arr(i).FID1, result_arr(i).LOW1, result_arr(i).HIGH1, result_arr(i).FNO2, result_arr(i).FID2, result_arr(i).LOW2, result_arr(i).HIGH2,result_arr(i).TERM_FNO, result_arr(i).TERM_FID, result_arr(i).TERM_LOW, result_arr(i).TERM_HIGH) );
			IF (result_arr(i).TERM_FNO IS NULL) THEN
				RAISE OUT_OF_RANGE;
			END IF;
		END LOOP;
	--ELSE -- else when Empty result
		--PIPE ROW(AG_CORECOUNT_ROW2( v_fFNO, v_fFID, -1, -1, v_tFNO, v_tFID, -1, -1,-1, -1, -1, -1) );

	END IF;

	EXCEPTION  -- exception handlers begin
		WHEN OUT_OF_RANGE THEN
			PIPE ROW(AG_CORECOUNT_ROW2( v_fFNO, v_fFID, -1, -1, v_tFNO, v_tFID, -1, -1,-1, -1, -1, -1) );
		WHEN OTHERS THEN  -- handles all other errors
			PIPE ROW(AG_CORECOUNT_ROW2( v_fFNO, v_fFID, -1, -1, v_tFNO, v_tFID, -1, -1,-1, -1, -1, -1) );
END;
/
