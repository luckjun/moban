using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Numerics;

namespace APP
{
    public class simHash
    {
        public int hashbits;
        public BigInteger hash;
        
        public simHash(string source, int hashbits = 64)
        {
            this.hashbits = hashbits;
            this.hash = this.simhash1(source.Split(new char[5] { ' ', '\t', ',', '.', '。' }));
        }

        public simHash(int hashbits = 64)
        {
            this.hashbits = hashbits;
        }

        public BigInteger _string_hash(string v)
        {
            BigInteger x = 0, m, mask;
            if (v == "" || v == null || v.Length == 0)
                return 0;
            else
            {
                x = ((long)v[0]) << 7;
                m = 1000003;
                BigInteger BigOne = 1;
                mask = (BigOne << this.hashbits) - 1;
                for (int i = 0; i < v.Length; i++)
                {
                    x = ((x * m) ^ ((long)v[i])) & mask;
                }
                x ^= v.Length;
                if (x == -1)
                    x = -2;
                // Console.WriteLine(x);
                return x;
            }
        }

        public BigInteger simhash1(string[] tokens)
        {
            int[] v = new int[this.hashbits];
            for (int i = 0; i < this.hashbits; i++)
                v[i] = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                BigInteger hashLong = _string_hash(tokens[i]);
                if (hashLong != 0)
                {
                    BigInteger bitmash = 0;
                    BigInteger bigOne = 1;
                    for (int k = 0; k < this.hashbits; k++)
                    {
                        bitmash = bigOne << k;
                        if ((hashLong & bitmash) > 0)
                        {
                            v[k] += 1;
                        }
                        else
                        {
                            v[k] -= 1;
                        }
                    }

                }
            }

            BigInteger fingerprint = 0;
            BigInteger BigOne = new BigInteger(1);
            for (int i = 0; i < this.hashbits; i++)
            {
                Console.Write(v[i].ToString() + " ");
                if (v[i] >= 0)
                    fingerprint += BigOne << i;
            }

            return fingerprint;
        }

        public int hamming_distance(simHash other_hash)
        {
            BigInteger BigOne = 1;
            BigInteger x = (this.hash ^ other_hash.hash) & ((BigOne << this.hashbits) - 1);
            int tot = 0;
            while (x > 0)
            {
                tot += 1;
                x &= x - 1;
            }
            return tot;
        }

        public int hamming_distance(string other_hash_string)
        {
            BigInteger BigOne = 1;
            BigInteger otherBigInt = BigInteger.Parse(other_hash_string);
            BigInteger x = (this.hash ^ otherBigInt) & ((BigOne << this.hashbits) - 1);
            int tot = 0;
            while (x > 0)
            {
                tot += 1;
                x &= x - 1;
            }
            return tot;
        }

        public int hamming_distance(string source_hash_string, string other_hash_string)
        {
            BigInteger BigOne = 1;
            BigInteger sourceBigInt = BigInteger.Parse(source_hash_string);
            BigInteger otherBigInt = BigInteger.Parse(other_hash_string);
            BigInteger x = (sourceBigInt ^ otherBigInt) & ((BigOne << this.hashbits) - 1);
            int tot = 0;
            while (x > 0)
            {
                tot += 1;
                x &= x - 1;
            }
            return tot;
        }

    }
}
