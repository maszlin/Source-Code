-- test main joint
select a.g3e_fid, c.exc_abb from gc_nr_connect a, gc_splice b, gc_netelem c where a.in_fid = b.g3e_fid and c.g3e_fid = b.g3e_fid and b.splice_class = 'Main Joint';


select a.g3e_fid, c.exc_abb, b.itface_code from gc_nr_connect a, gc_splice b, gc_netelem c where a.in_fno = '10300' and a.in_fid = b.g3e_fid and c.g3e_fid = b.g3e_fid;


SELECT A.* FROM GC_CBL A, GC_NETELEM B, GC_NR_CONNECT C 
WHERE A.ITFACE_CODE = '004' AND CABLE_CODE = 'D1' AND B.EXC_ABB = 'BGI' AND B.G3E_FID = A.G3E_FID AND C.IN_FNO = 10300 AND C.G3E_FID = A.G3E_FID

SELECT A.CABLE_CODE, A.G3E_FID, A.TOTAL_SIZE, A.EFFECTIVE_PAIRS FROM GC_CBL A, GC_NETELEM B, GC_NR_CONNECT C 
WHERE A.ITFACE_CODE = '021' AND B.EXC_ABB = 'BGI' AND B.G3E_FID = A.G3E_FID AND C.IN_FNO = 10300 AND C.G3E_FID = A.G3E_FID;