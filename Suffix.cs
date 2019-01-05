using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public enum Semantics
    {	
		// NOUN-BASED
        PLU,  // -lar, -ler 									Plural Suffix
		ACC,  // -ı, -i, -u, -ü
		//		 -yı, -yi, -yu, -yü, -nı, -ni, -nu, -nü  		Accusative Case
		DAT,  // -a, -e 										Dative Case
		LOC,  // -da, -de, -ta, -te 							Locative Case
		ABL,  // -dan, -den, -tan, -ten, -ndan, -nden			Ablative Case
		PSF,  // -m, -ım, -im, -um, -üm 						Possessive Case  - Singular First
		PSS,  // -n, -ın, -in, -un, -ün 						Possessive Case  - Singular Second
		PST,  // -ı, -i, -u, -ü, -sı, -si, -su, -sü,            Possessive Case  - Singular Third
		PPF,  // -mız, -miz, -muz, -müz, 
        //       -ımız, -imiz, -umuz, -ümüz                     Possessive Case  - Plural First
		PPS,  // -nız, -niz, -nuz, -nüz 						
        //       -ınız, -iniz, -unuz, -ünüz                     Possessive Case  - Plural Second
        PPT,  // -ları, -leri 									Possessive Case  - Plural Third
		ISF,  // -yım, -yim, -yum, -yüm, -ım, -im, -um, -üm		Indicative Case  - Singular First
		ISS,  // -sın, -sin, -sun, -sün							Indicative Case  - Singular Second
		IST,  // -dır, -dir, -dur, -dür, -tır, -tir, -tur, -tür	Indicative Case  - Singular Third
		IPF,  // -yız, -yiz, -yuz, -yüz, -ız, -iz, -uz, -üz		Indicative Case  - Plural First
		IPS,  // -sınız, -siniz, -sunuz, -sünüz					Indicative Case  - Plural Second
		IPT,  // -dırlar, -dirler, -durlar, -dürler,  
		//		 -tırlar, -tirler, -turlar, -türler				Indicative Case  - Plural Third
		KSF,  // -ydım, -ydim, -ydum, -ydüm, 
		//		 -dım, -dim, -dum, -düm							Known Past Case  - Singular First
		KSS,  // -ydın, -ydin, -ydun, -ydün,
		// 		 -dın, -din, -dun, -dün							Known Past Case  - Singular Second
		KST,  // -dı, -di, -du, -dü, -tı, -ti, -tu, -tü,
		//		 -ydı, -ydi, -ydu, -ydü				 			Known Past Case  - Singular Third
		KPF,  // -ydık, -ydik, -yduk, -ydük, 		
		//		 -dık, -dik, -duk, -dük, -tık, -tik, -tuk, -tük	Known Past Case  - Plural First
		KPS,  // -dınız, -diniz, -dunuz, -dünüz
        //       -tınız, -tiniz, -tunuz, -tünüz,
        //       -ydınız, -ydiniz, -ydunuz, -ydünüz             Known Past Case  - Plural Second
		KPT,  // -dılar, -diler, -dular, -düler,  
        //		 -tılar, -tiler, -tular, -tüler			    	
        //       -ydılar, -ydiler, -ydular, -ydüler             Known Past Case  - Plural Third
		HSF,  // -ymışım, -ymişim, -ymuşum, -ymüşüm, 
		//		 -mışım, -mişim, -muşum, -müşüm					Heard Past Case  - Singular First
		HSS,  // -ymışsın, -ymişsin, -ymuşsun, -ymüşsün
		// 		 -mışsın, -mişsin, -muşsun, -müşsün				Heard Past Case  - Singular Second
		HST,  // -mış, -miş, -muş, -müş,
		//		 -ymış, -ymiş, -ymuş, -ymüş			            Heard Past Case  - Singular Third
		HPF,  // -ymışız, -ymişiz, -ymuşuz, -ymüşüz
		// 		 -mışız, -mişiz, -muşuz, -müşüz					Heard Past Case  - Plural First
		HPS,  // -ymışsınız, -ymişsiniz, -ymuşsunuz, -ymüşsünüz
		// 		 -mışsınız, -mişsiniz, -muşsunuz, -müşsünüz		Heard Past Case  - Plural Second
		HPT,  // -mışlar, -mişler, -muşlar, -müşler
        //       -ymışlar, -ymişler, -ymuşlar, -ymüşler         Heard Past Case  - Plural Third
		CSF,  // -ysem, -ysam, -sem, -sam 						Conditional Case - Singular First
		CSS,  // -ysen, -ysan, -san, -sen						Conditional Case - Singular Second
		CST,  // -yse, -ysa, -se, -sa							Conditional Case - Singular Third
		CPF,  // -ysek, -ysak, -sek, -sak						Conditional Case - Plural First
		CPS,  // -ysanız, -yseniz, -sanız,- seniz				Conditional Case - Plural Second
		CPT,  // -ysalar, -yseler, -salar, -seler  				Conditional Case - Plural Third
		VIA,  // -la, -le, -yla, -yle							Instrumental Case
		RPN,  // -ki											Relative Pronoun
		EQU,  // -ca, -ça, -ce, -çe								Fairness Suffix
		
		// VERB-BASED
		LEX,  // -mak, -mek										Lexical Form
		NEG,  // -ma, -me										Negation
		KSFV, // -dım, -dim, -dum, -düm, -tım, -tim, -tum, -tüm Known Past Case  - Singular First
		KSSV, // -dın, -din, -dun, -dün, -tın, -tin, -tun, -tün Known Past Case  - Singular Second
		KSTV, // -dı, -di, -du, -dü, -tı, -ti, -tu, -tü 		Known Past Case  - Singular Third
		KPFV, // -dık, -dik, -duk, -dük, -tık, -tik, -tuk, -tük Known Past Case  - Plural First
		KPSV, // -dınız, -diniz, -dunuz, -dünüz, 
		//		 -tınız, -tiniz, -tunuz, -tünüz					Known Past Case  - Plural Second
		KPTV, // -dılar, -diler, -dular, -düler, 
		//		 -tılar, -tiler, -tular, -tüler 				Known Past Case  - Plural Third
		HSFV, // -mışım, -mişim, -muşum, -müşüm				    Heard Past Case  - Singular First
		HSSV, // -mışsın, -mişsin, -muşsun, -müşsün			    Heard Past Case  - Singular Second
		HSTV, // -mış, -miş, -muş, -müş							Heard Past Case  - Singular Third
		HPFV, // -mışız, -mişiz, -muşuz, -müşüz 				Heard Past Case  - Plural First
		HPSV, // -mışsınız, -mişsiniz, -muşsunuz, -müşsünüz 	Heard Past Case  - Plural Second
		HPTV, // -mışlar, -mişler, -muşlar, -müşler 			Heard Past Case  - Plural Third
		COSF, // -yorum, -ıyorum, -iyorum, -uyorum, -üyorum		Continuous Case  - Singular First
		COSS, // -yorsun, -ıyorsun, -iyorsun, 
        //       -uyorsun, -üyorsun                             Continuous Case  - Singular Second
		COST, // -yor, -ıyor, -iyor, -uyor, -üyor				Continuous Case  - Singular Third
		COPF, // -yoruz, -ıyoruz, -iyoruz, -uyoruz, -üyoruz		Continuous Case  - Plural First
		COPS, // -yorsunuz, -ıyorsunuz, 
        //      -iyorsunuz, -uyorsunuz, -üyorsunuz	            Continuous Case  - Plural Second
		COPT, // -yorlar, -ıyorlar, -iyorlar,
        //       -uyorlar, -üyorlar								Continuous Case  - Plural Third
		FSF,  // -acağım, -eceğim, -yacağım, -yeceğim			Future Case 	 - Singular First
		FSS,  // -acaksın, -eceksin, -yacaksın, -yeceksin		Future Case 	 - Singular Second
		FST,  // -acak, -ecek, -yacak, -yecek					Future Case 	 - Singular Third
		FPF,  // -acağız, -eceğiz, -yacağız, -yeceğiz			Future Case 	 - Plural First
		FPS,  // -acaksınız, -eceksiniz, 
		//		 -yacaksınız, -yeceksiniz						Future Case 	 - Plural Second
		FPT,  // -acaklar, -ecekler, -yacaklar, -yecekler		Future Case 	 - Plural Third
		PRSF, // -rım, -rim, -rum, -ırım, -irim,
        //       -urum, -ürüm, -arım, -erim                 	Present Case     - Singular First
		PRSS, // -rsın, -ırsın, -irsin, 
		//		 -ursun, -ürsün, -arsın, -ersin 				Present Case     - Singular Second
		PRST, // -r, -ır, -ir, -ur, -ür, -ar, -er				Present Case     - Singular Third
		PRPF, // -rız, -riz, -ruz, -ırız, 
        //       -iriz, -uruz, -ürüz, -arız, -eriz	            Present Case     - Plural First
		PRPS, // -rsınız, -ırsınız, -irsiniz, 
		//		 -ursunuz, -ürsünüz, -arsınız, -ersiniz 		Present Case     - Plural Second
		PRPT, // -rlar, -ırlar, -irler, 
		//		 -urlar, -ürler, -arlar, -erler					Present Case     - Plural Third
		NGVF, // -amam, -emem, -yamam, -yemem					Negation Suffix of -bil - Singular First
        NGVS, // -amazsın, -emezsin, -yamazsın, -yemezsin		Negation Suffix of -bil - Singular Second
        NGVT, // -amaz, -emez, -yamaz, -yemez					Negation Suffix of -bil - Singular Third
        NGPF, // -amayız, -emeyiz, -yamayız, -yemeyiz			Negation Suffix of -bil - Plural First
        NGPS, // -amazsınız, -emezsiniz, 
        //       -yamazsınız, -yemezsiniz                       Negation Suffix of -bil - Plural Second
        NGPT, // -amazlar, -emezlar, -yamazlar, -yemezler		Negation Suffix of -bil - Plural Third
        NPSF, // -mam, -mem										Present Case     - Singular First Negation
		NPSS, // -mazsın, -mezsin								Present Case     - Singular Second Negation
		NPST, // -maz, -mez										Present Case     - Singular Third Negation
		NPPF, // -mayız, -meyiz									Present Case     - Plural First Negation
		NPPS, // -mazsınız, -mezsiniz							Present Case     - Plural Second Negation
		NPPT, // -mazlar, -mezler								Present Case     - Plural Third Negation
		CSFV, // -sem, -sam 									Conditional Case - Singular First
		CSSV, // -sen, -san										Conditional Case - Singular Second
		CSTV, // -se, -sa										Conditional Case - Singular Third
		CPFV, // -sek, -sak										Conditional Case - Plural First
		CPSV, // -sanız, -seniz									Conditional Case - Plural Second
		CPTV, // -larsa, -lerse					  				Conditional Case - Plural Third
		RESF, // -ayım, -eyim, -yayım, -yeyim 					Request Case 	 - Singular First
		RESS, // -asın, -esin, -yasın, -yesin					Request Case 	 - Singular Second
		REST, // -a, -e, -ya, -ye								Request Case	 - Singular Third
		REPF, // -alım, -elim, -yalım, -yelim					Request Case     - Plural First
		REPS, // -asınız, -esiniz, -yasınız, -yesiniz			Request Case     - Plural Second
		REPT, // -alar, -eler, -yalar, -yeler					Request Case	 - Plural Third
		NESF, // -malıyım, -meliyim								Necessity Case 	 - Singular First
		NESS, // -malısın, -melisin								Necessity Case 	 - Singular Second
		NEST, // -malı, -meli									Necessity Case	 - Singular Third
		NEPF, // -malıyız, -meliyiz								Necessity Case   - Plural First
		NEPS, // -malısınız, -melisiniz							Necessity Case   - Plural Second
		NEPT, // -malılar, -meliler								Necessity Case	 - Plural Third
		MPST, // -sın, -sin, -sun, -sün							Imperative Case  - Singular Third
		MPPS, // -ın, -in, -un, -ün, -ınız, -iniz, -unuz, -ünüz	Imperative Case  - Singular Third
		MPPT, // -sınlar, -sinler, -sunlar, -sünler				Imperative Case  - Singular Third

        NULL
    }

    public class Suffix
    {
        private string syntax;
        public string Syntax { get { return syntax; } set { syntax = value; } }
        private Semantics semantics;
        public Semantics Semantics { get { return semantics; } set { semantics = value; } }
        private short polarity;
        public short Polarity { get { return polarity; } set { polarity = value; } }

        public Suffix(string syntax, Semantics semantics, short polarity)
        {
            this.syntax = syntax;
            this.semantics = semantics;
            this.polarity = polarity;
        }

        public override string ToString()
        {
            return syntax + " " + semantics + " " + polarity;
        }
        public static Suffix FindSuffixOR(SentenceWord sw, params Semantics[] s)
        {
            return Array.Find(sw.Suffixes, sem => {
                for (int i = 0; i < s.Length; i++)
                {
                    if (sem.Semantics == s[i])
                    {
                        return true;
                    }
                }
                return false;
            });
        }
        public static Suffix FindSuffixAND(SentenceWord sw, params Semantics[] s)
        {
            int count = 0;
            return Array.Find(sw.Suffixes, sem => {               
                for (int i = 0; i < s.Length; i++)
                {
                    if (sem.Semantics == s[i])
                        count++;
                }
                if (count == s.Length)
                    return true;
                else
                    return false;
            });
        }

        public static int PolarityAND(params int[] polarities)
        {
            if (polarities.Length > 1)
            {
                int result = 255;
                if (polarities[0] == -1 && polarities[1] == -1)
                    result = -1;
                if (polarities[0] == 0 && polarities[1] == 0)
                    result = 0;
                if (polarities[0] == 1 && polarities[1] == 1)
                    result = 1;
                if ((polarities[0] == 0 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 0))
                    result = -1;
                if ((polarities[0] == 1 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 1))
                    result = -1;
                if ((polarities[0] == 0 && polarities[1] == 1) || (polarities[0] == 1 && polarities[1] == 0))
                    result = 1;
                for (int i = 2; i < polarities.Length; i++)
                {
                    if (result == -1 && polarities[i] == -1)
                        result = -1;
                    if (result == 0 && polarities[i] == 0)
                        result = 0;
                    if (result == 1 && polarities[i] == 1)
                        result = 1;
                    if ((result == 0 && polarities[i] == -1) || (result == -1 && polarities[i] == 0))
                        result = -1;
                    if ((result == 1 && polarities[i] == -1) || (result == -1 && polarities[i] == 1))
                        result = -1;
                    if ((result == 0 && polarities[i] == 1) || (result == 1 && polarities[i] == 0))
                        result = 1;
                }
                return result;
            }
            else if (polarities.Length == 1)
                return polarities[0];
            else
                return 0;
        }

        public static int PolarityXOR(params int[] polarities)
        {
            if (polarities.Length > 1)
            {
                int result = 255;
                if (polarities[0] == -1 && polarities[1] == -1)
                    result = -1;
                if (polarities[0] == 0 && polarities[1] == 0)
                    result = 0;
                if (polarities[0] == 1 && polarities[1] == 1)
                    result = -1;
                if ((polarities[0] == 0 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 0))
                    result = -1;
                if ((polarities[0] == 1 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 1))
                    result = 1;
                if ((polarities[0] == 0 && polarities[1] == 1) || (polarities[0] == 1 && polarities[1] == 0))
                    result = 1;
                for (int i = 2; i < polarities.Length; i++)
                {
                    if (result == -1 && polarities[i] == -1)
                        result = -1;
                    if (result == 0 && polarities[i] == 0)
                        result = 0;
                    if (result == 1 && polarities[i] == 1)
                        result = -1;
                    if ((result == 0 && polarities[i] == -1) || (result == -1 && polarities[i] == 0))
                        result = -1;
                    if ((result == 1 && polarities[i] == -1) || (result == -1 && polarities[i] == 1))
                        result = 1;
                    if ((result == 0 && polarities[i] == 1) || (result == 1 && polarities[i] == 0))
                        result = 1;
                }
                return result;
            }
            else if (polarities.Length == 1)
                return polarities[0];
            else
                return 0;
        }

        public static int PolarityOR(params int[] polarities)
        {
            if (polarities.Length > 1)
            {
                int result = 255;
                if (polarities[0] == -1 && polarities[1] == -1)
                    result = -1;
                if (polarities[0] == 0 && polarities[1] == 0)
                    result = 0;
                if (polarities[0] == 1 && polarities[1] == 1)
                    result = 1;
                if ((polarities[0] == 0 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 0))
                    result = -1;
                if ((polarities[0] == 1 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 1))
                    result = 1;
                if ((polarities[0] == 0 && polarities[1] == 1) || (polarities[0] == 1 && polarities[1] == 0))
                    result = 1;
                for (int i = 2; i < polarities.Length; i++)
                {
                    if (result == -1 && polarities[i] == -1)
                        result = -1;
                    if (result == 0 && polarities[i] == 0)
                        result = 0;
                    if (result == 1 && polarities[i] == 1)
                        result = 1;
                    if ((result == 0 && polarities[i] == -1) || (result == -1 && polarities[i] == 0))
                        result = -1;
                    if ((result == 1 && polarities[i] == -1) || (result == -1 && polarities[i] == 1))
                        result = 1;
                    if ((result == 0 && polarities[i] == 1) || (result == 1 && polarities[i] == 0))
                        result = 1;
                }
                return result;
            }
            else if (polarities.Length == 1)
                return polarities[0];
            else
                return 0;
        }

        public static int PolaritySummation(params int[] polarities)
        {
            if (polarities.Length > 1)
            {
                int result = 255;
                if (polarities[0] == -1 && polarities[1] == -1)
                    result = 1;
                if (polarities[0] == 0 && polarities[1] == 0)
                    result = 0;
                if (polarities[0] == 1 && polarities[1] == 1)
                    result = 1;
                if ((polarities[0] == 0 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 0))
                    result = -1;
                if ((polarities[0] == 1 && polarities[1] == -1) || (polarities[0] == -1 && polarities[1] == 1))
                    result = -1;
                if ((polarities[0] == 0 && polarities[1] == 1) || (polarities[0] == 1 && polarities[1] == 0))
                    result = 1;
                for (int i = 2; i < polarities.Length; i++)
                {
                    if (result == -1 && polarities[i] == -1)
                        result = 1;
                    if (result == 0 && polarities[i] == 0)
                        result = 0;
                    if (result == 1 && polarities[i] == 1)
                        result = 1;
                    if ((result == 0 && polarities[i] == -1) || (result == -1 && polarities[i] == 0))
                        result = -1;
                    if ((result == 1 && polarities[i] == -1) || (result == -1 && polarities[i] == 1))
                        result = -1;
                    if ((result == 0 && polarities[i] == 1) || (result == 1 && polarities[i] == 0))
                        result = 1;
                }
                return result;
            }
            else if (polarities.Length == 1)
                return polarities[0];
            else
                return 0;
        }

        public static int PolaritySummation(params Suffix[] suffixes)
        {
            if (suffixes.Length > 1)
            {
                int result = 255;
                if (suffixes[0].Polarity == -1 && suffixes[1].Polarity == -1)
                    result = 1;
                if (suffixes[0].Polarity == 0 && suffixes[1].Polarity == 0)
                    result = 0;
                if (suffixes[0].Polarity == 1 && suffixes[1].Polarity == 1)
                    result = 1;
                if ((suffixes[0].Polarity == 0 && suffixes[1].Polarity == -1) || (suffixes[0].Polarity == -1 && suffixes[1].Polarity == 0))
                    result = -1;
                if ((suffixes[0].Polarity == 1 && suffixes[1].Polarity == -1) || (suffixes[0].Polarity == -1 && suffixes[1].Polarity == 1))
                    result = -1;
                if ((suffixes[0].Polarity == 0 && suffixes[1].Polarity == 1) || (suffixes[0].Polarity == 1 && suffixes[1].Polarity == 0))
                    result = 1;
                for (int i = 2; i < suffixes.Length; i++)
                {
                    if (result == -1 && suffixes[i].Polarity == -1)
                        result = 1;
                    if (result == 0 && suffixes[i].Polarity == 0)
                        result = 0;
                    if (result == 1 && suffixes[i].Polarity == 1)
                        result = 1;
                    if ((result == 0 && suffixes[i].Polarity == -1) || (result == -1 && suffixes[i].Polarity == 0))
                        result = -1;
                    if ((result == 1 && suffixes[i].Polarity == -1) || (result == -1 && suffixes[i].Polarity == 1))
                        result = -1;
                    if ((result == 0 && suffixes[i].Polarity == 1) || (result == 1 && suffixes[i].Polarity == 0))
                        result = 1;
                }
                return result;
            }
            else if (suffixes.Length == 1)
                return suffixes[0].Polarity;
            else
                return 0;
        }
    }



}