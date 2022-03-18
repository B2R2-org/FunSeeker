import os, sys, subprocess

def collect(lines):
    addrs = set()
    in_elem = False
    for line in lines:
        if 'Abbrev Number' in line:
            line = line.split('Abbrev Number:')[1]
            if '(' in line:
                tag = line.split('(')[1].split(')')[0]
                if tag == 'DW_TAG_subprogram':
                    in_elem = True
                else:
                    in_elem = False
            else:
                in_elem = False
        elif in_elem:
            if 'DW_AT_low_pc' in line:
                addr = line.split(':')[1]
                addr = addr.strip()
                addr = int(addr, 16)
                addrs.add(addr)
    return addrs

def main(bin_path, out_path):
    p = subprocess.Popen(['readelf', '--debug-dump=info', bin_path], stdout=subprocess.PIPE)
    out, _ = p.communicate()
    out = out.decode()
    lines = out.split('\n')
    addrs = collect(lines)
    addrs = list(addrs)
    addrs.sort()

    with open(out_path, 'w') as f:
        for addr in addrs:
            f.write('%x\n' % addr)

if __name__ == '__main__':
    bin_path = sys.argv[1]
    out_path = sys.argv[2]
    main(bin_path, out_path)
