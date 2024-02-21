using System.Collections;
using UnityEngine.Networking;
using GameNetcodeStuff;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hax;

readonly struct TranslateRequest(string source, string target, string text) {
    [JsonProperty("source")]
    internal string Source { get; } = source;

    [JsonProperty("target")]
    internal string Target { get; } = target;

    [JsonProperty("text")]
    internal string Text { get; } = text;
}

internal static partial class Helper {
    static Lazy<Dictionary<string, string>> FLORES200 { get; } = new(() => new Dictionary<string, string>() {
        { "acehnese", "ace_Latn" },
        { "mesopotamian", "acm_Arab" },
        { "tunisian", "aeb_Arab" },
        { "afrikaans", "afr_Latn" },
        { "levantine", "ajp_Arab" },
        { "akan", "aka_Latn" },
        { "amharic", "amh_Ethi" },
        { "arabic", "arb_Arab" },
        { "najdi", "ars_Arab" },
        { "moroccan", "ary_Arab" },
        { "egyptian", "arz_Arab" },
        { "assamese", "asm_Beng" },
        { "asturian", "ast_Latn" },
        { "awadhi", "awa_Deva" },
        { "aymara", "ayr_Latn" },
        { "azerbaijani", "azj_Latn" },
        { "bashkir", "bak_Cyrl" },
        { "bambara", "bam_Latn" },
        { "balinese", "ban_Latn" },
        { "belarusian", "bel_Cyrl" },
        { "bemba", "bem_Latn" },
        { "bengali", "ben_Beng" },
        { "bhojpuri", "bho_Deva" },
        { "banjar", "bjn_Latn" },
        { "tibetan", "bod_Tibt" },
        { "bosnian", "bos_Latn" },
        { "buginese", "bug_Latn" },
        { "bulgarian", "bul_Cyrl" },
        { "catalan", "cat_Latn" },
        { "cebuano", "ceb_Latn" },
        { "czech", "ces_Latn" },
        { "chokwe", "cjk_Latn" },
        { "welsh", "cym_Latn" },
        { "danish", "dan_Latn" },
        { "german", "deu_Latn" },
        { "dinka", "dik_Latn" },
        { "dyula", "dyu_Latn" },
        { "dzongkha", "dzo_Tibt" },
        { "greek", "ell_Grek" },
        { "english", "eng_Latn" },
        { "esperanto", "epo_Latn" },
        { "estonian", "est_Latn" },
        { "basque", "eus_Latn" },
        { "ewe", "ewe_Latn" },
        { "faroese", "fao_Latn" },
        { "fijian", "fij_Latn" },
        { "finnish", "fin_Latn" },
        { "fon", "fon_Latn" },
        { "french", "fra_Latn" },
        { "friulian", "fur_Latn" },
        { "fulfulde", "fuv_Latn" },
        { "gaelic", "gla_Latn" },
        { "irish", "gle_Latn" },
        { "galician", "glg_Latn" },
        { "guarani", "grn_Latn" },
        { "gujarati", "guj_Gujr" },
        { "haitian", "hat_Latn" },
        { "hausa", "hau_Latn" },
        { "hebrew", "heb_Hebr" },
        { "hindi", "hin_Deva" },
        { "chhattisgarhi", "hne_Deva" },
        { "croatian", "hrv_Latn" },
        { "hungarian", "hun_Latn" },
        { "armenian", "hye_Armn" },
        { "igbo", "ibo_Latn" },
        { "ilocano", "ilo_Latn" },
        { "indonesian", "ind_Latn" },
        { "icelandic", "isl_Latn" },
        { "italian", "ita_Latn" },
        { "javanese", "jav_Latn" },
        { "japanese", "jpn_Jpan" },
        { "kabyle", "kab_Latn" },
        { "jingpho", "kac_Latn" },
        { "kamba", "kam_Latn" },
        { "kannada", "kan_Knda" },
        { "kashmiri", "kas_Arab" },
        { "georgian", "kat_Geor" },
        { "kanuri", "knc_Latn" },
        { "kazakh", "kaz_Cyrl" },
        { "kabiyè", "kbp_Latn" },
        { "kabuverdianu", "kea_Latn" },
        { "khmer", "khm_Khmr" },
        { "kikuyu", "kik_Latn" },
        { "kinyarwanda", "kin_Latn" },
        { "kyrgyz", "kir_Cyrl" },
        { "kimbundu", "kmb_Latn" },
        { "kurdish", "kmr_Latn" },
        { "kikongo", "kon_Latn" },
        { "korean", "kor_Hang" },
        { "lao", "lao_Laoo" },
        { "ligurian", "lij_Latn" },
        { "limburgish", "lim_Latn" },
        { "lingala", "lin_Latn" },
        { "lithuanian", "lit_Latn" },
        { "lombard", "lmo_Latn" },
        { "latgalian", "ltg_Latn" },
        { "luxembourgish", "ltz_Latn" },
        { "luba-kasai", "lua_Latn" },
        { "ganda", "lug_Latn" },
        { "luo", "luo_Latn" },
        { "mizo", "lus_Latn" },
        { "latvian", "lvs_Latn" },
        { "magahi", "mag_Deva" },
        { "maithili", "mai_Deva" },
        { "malayalam", "mal_Mlym" },
        { "marathi", "mar_Deva" },
        { "minangkabau", "min_Latn" },
        { "macedonian", "mkd_Cyrl" },
        { "malagasy", "plt_Latn" },
        { "maltese", "mlt_Latn" },
        { "meitei", "mni_Beng" },
        { "mongolian", "khk_Cyrl" },
        { "mossi", "mos_Latn" },
        { "maori", "mri_Latn" },
        { "burmese", "mya_Mymr" },
        { "dutch", "nld_Latn" },
        { "nynorsk", "nno_Latn" },
        { "bokmål", "nob_Latn" },
        { "nepali", "npi_Deva" },
        { "nuer", "nus_Latn" },
        { "nyanja", "nya_Latn" },
        { "occitan", "oci_Latn" },
        { "oromo", "gaz_Latn" },
        { "odia", "ory_Orya" },
        { "pangasinan", "pag_Latn" },
        { "panjabi", "pan_Guru" },
        { "papiamento", "pap_Latn" },
        { "persian", "pes_Arab" },
        { "polish", "pol_Latn" },
        { "portuguese", "por_Latn" },
        { "dari", "prs_Arab" },
        { "pashto", "pbt_Arab" },
        { "ayacuchoquechua", "quy_Latn" },
        { "romanian", "ron_Latn" },
        { "rundi", "run_Latn" },
        { "russian", "rus_Cyrl" },
        { "sango", "sag_Latn" },
        { "sanskrit", "san_Deva" },
        { "santali", "sat_Olck" },
        { "sicilian", "scn_Latn" },
        { "shan", "shn_Mymr" },
        { "sinhala", "sin_Sinh" },
        { "slovak", "slk_Latn" },
        { "slovenian", "slv_Latn" },
        { "samoan", "smo_Latn" },
        { "shona", "sna_Latn" },
        { "sindhi", "snd_Arab" },
        { "somali", "som_Latn" },
        { "sotho", "sot_Latn" },
        { "spanish", "spa_Latn" },
        { "toskalbanian", "als_Latn" },
        { "sardinian", "srd_Latn" },
        { "serbian", "srp_Cyrl" },
        { "swati", "ssw_Latn" },
        { "sundanese", "sun_Latn" },
        { "swedish", "swe_Latn" },
        { "swahili", "swh_Latn" },
        { "silesian", "szl_Latn" },
        { "tamil", "tam_Taml" },
        { "tatar", "tat_Cyrl" },
        { "telugu", "tel_Telu" },
        { "tajik", "tgk_Cyrl" },
        { "tagalog", "tgl_Latn" },
        { "thai", "tha_Thai" },
        { "tigrinya", "tir_Ethi" },
        { "tamasheq", "taq_Latn" },
        { "tokpisin", "tpi_Latn" },
        { "tswana", "tsn_Latn" },
        { "tsonga", "tso_Latn" },
        { "turkmen", "tuk_Latn" },
        { "tumbuka", "tum_Latn" },
        { "turkish", "tur_Latn" },
        { "twi", "twi_Latn" },
        { "tamazight", "tzm_Tfng" },
        { "uyghur", "uig_Arab" },
        { "ukrainian", "ukr_Cyrl" },
        { "umbundu", "umb_Latn" },
        { "urdu", "urd_Arab" },
        { "uzbek", "uzn_Latn" },
        { "venetian", "vec_Latn" },
        { "vietnamese", "vie_Latn" },
        { "waray", "war_Latn" },
        { "wolof", "wol_Latn" },
        { "xhosa", "xho_Latn" },
        { "yiddish", "ydd_Hebr" },
        { "yoruba", "yor_Latn" },
        { "yue", "yue_Hant" },
        { "chinese", "zho_Hans" },
        { "malay", "zsm_Latn" },
        { "zulu", "zul_Latn" },
    });

    static IEnumerator Translate(TranslateRequest request) {
        if (Helper.LocalPlayer is not PlayerControllerB player) yield break;

        using UnityWebRequest www = UnityWebRequest.Post(
            "https://winstxnhdw-nllb-api.hf.space/api/v2/translate",
            JsonConvert.SerializeObject(request),
            "application/json"
        );

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            State.TranslateDetachedState = null;
            Helper.HUDManager?.AddTextToChatOnServer(request.Text, player.PlayerIndex());
            Chat.Print("Translation server is down!");
        }

        else {
            _ = Helper.HUDManager?.Reflect().InvokeInternalMethod("AddPlayerChatMessageServerRpc", www.downloadHandler.text.Trim(), player.PlayerIndex());
        }
    }

    internal static void Translate(string? sourceLanguage, string? targetLanguage, string? text) {
        if (string.IsNullOrWhiteSpace(text)) return;

        string? source = Helper.FuzzyMatch(sourceLanguage?.ToLower(), Helper.FLORES200.Value.Keys);
        string? target = Helper.FuzzyMatch(targetLanguage?.ToLower(), Helper.FLORES200.Value.Keys);

        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target)) {
            Chat.Print("Failed to find the language!");
            return;
        }

        TranslateRequest request = new(
            Helper.FLORES200.Value[source],
            Helper.FLORES200.Value[target],
            text
        );

        Helper.CreateComponent<AsyncBehaviour>().Init(() => Helper.Translate(request));
    }
}
