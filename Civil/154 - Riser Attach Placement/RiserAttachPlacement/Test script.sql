select * from G3E_JOB

select * from AG_BDY_PAD where bdy_fid=11100106

select * from GC_NETELEM

select * from GC_BND_P

select G3E_FID, G3E_FNO from GC_BND_P WHERE G3E_FNO=24000 AND FEATURE_TYPE='EXC' ORDER BY G3E_ID

select * from gc_netelem N where UPPER(JOB_ID)='MIG_JOB' AND UPPEr(FEATURE_STATE)='PPF' AND G3E_FNO=10300 AND EXISTS (SELECT * FROM AG_BDY_PAD A WHERE A.fea_fid=n.g3e_fid AND A.BDY_FID=10961534)

select count(*) from gc_netelem N where G3E_FNO=10300 AND EXISTS (SELECT * FROM AG_BDY_PAD A WHERE A.fea_fid=n.g3e_fid AND A.BDY_FID=10961534)

select * from b$gc_netelem where g3e_fid=11151338
select exc_abb from gc_netelem where g3e_fno=6000
select * from gc_netelem where exc_abb='USJ1'

select * from g3e_feature where UPPER(g3e_username) LIKE '%EXCHANGE%'

update gc_netelem set job_id='hazman2' where g3e_fid=11151337

SELECT count(*) FROM AG_BDY_PAD A WHERE A.BDY_FID=10961534

select * from gc_netelem N where UPPER(JOB_ID)='MIG_JOB' AND UPPER(FEATURE_STATE)='PPF' AND G3E_FNO=10300 AND EXISTS (SELECT * FROM AG_BDY_PAD A WHERE A.fea_fid=n.g3e_fid AND A.BDY_FID=11100106) ORDER BY G3E_FID

