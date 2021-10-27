using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class TokenProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] TokenProgramIdBytes =
        {
            6, 221, 246, 225, 215, 101, 161, 147, 217, 203,
            225, 70, 206, 235, 121, 172, 28, 180, 133, 237,
            95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };

        private static readonly byte[] ExpectedTransferData =
        {
            3, 168, 97, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] ExpectedTransferCheckedData =
        {
            12, 168, 97, 0, 0, 0, 0, 0, 0, 2
        };

        private static readonly byte[] ExpectedInitializeMintData =
        {
            0, 2, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129,
            160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 1, 71, 105, 171, 151, 32, 75, 168, 63, 176,
            202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178
        };

        private static readonly byte[] ExpectedInitializeMultiSignatureData = { 2, 3 };

        private static readonly byte[] ExpectedMintToData =
        {
            7, 168, 97, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] ExpectedMintToCheckedData =
        {
            14, 168, 97, 0, 0, 0, 0, 0, 0,2
        };

        private static readonly byte[] ExpectedBurnData =
        {
            8, 168, 97, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] ExpectedBurnCheckedData =
        {
            15, 168, 97, 0, 0, 0, 0, 0, 0,2
        };

        private static readonly byte[] ExpectedInitializeAccountData = { 1 };

        private static readonly byte[] ExpectedApproveData = { 4, 168, 97, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] ExpectedApproveCheckedData = { 13, 168, 97, 0, 0, 0, 0, 0, 0, 2 };

        private static readonly byte[] ExpectedRevokeData = { 5 };

        private static readonly byte[] ExpectedSetAuthorityOwnerData =
        {
            6, 2, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityCloseData =
        {
            6, 3, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityFreezeData =
        {
            6, 1, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityMintData =
        {
            6, 0, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };

        private static readonly byte[] ExpectedCloseAccountData = { 9 };
        private static readonly byte[] ExpectedFreezeAccountData = { 10 };
        private static readonly byte[] ExpectedThawAccountData = { 11 };

        private const string InitializeMultisigMessage =
            "AwAJDEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyeLALNX+Hq5QvYpjBUrxcE6c1OPFtuOsWTs" +
            "RwZ22JTNv0sF4mdbv4FGc/JcD4qM+DJXE0k+DhmNmPu8MItrFyfgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAABqfVFxksXFEhjMlMPUrxf1ja7gibof1E49vZigAAAAC9PD4jUE81HRWVKjhuaeGhBDrUiRU" +
            "sQ8PRa6Gkh7BcAzbV0glem2ocQYDPKtsvb2P6eY+diK2RlCQbryCDiW9ENqhqvd4wlbvt2WLwsRs1GuOPhm" +
            "Rt728O9WHpObgVQ60+Im+a09G04MQPhepwoQn2VGuSmOoDsZvfRJ8im8hThYp3QXZN2eL1ihOJMfLOtOE0d" +
            "btnaKq58W0jnl+pjmXBBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F3IgtYUpVZyeIop" +
            "bd8eq6vQpgZ4iEky9O72oNRsOMzYJJil8tqxLyZCv3xaGw9O1hPoqUsFwShXE+aABQMCAAE0AAAAAJBLMwA" +
            "AAAAAYwEAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQoHAQQFBgcICQICAwMCAAI0AA" +
            "AAAGBNFgAAAAAAUgAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQoCAgRDAAp4sAs1f" +
            "4erlC9imMFSvFwTpzU48W246xZOxHBnbYlM2wBT7yGUtArURga4Avg+yhMwOEM69UaXYBPa+5CFN2YhDQsB" +
            "ABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string MintToMultisigMessage =
            "BQMFC0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyDpwtYu322rjxMK+ED8DutHhxOkdgN0Rl6/B7o" +
            "VsMMG69PD4jUE81HRWVKjhuaeGhBDrUiRUsQ8PRa6Gkh7BcAzbV0glem2ocQYDPKtsvb2P6eY+diK2RlCQbry" +
            "CDiW9EPiJvmtPRtODED4XqcKEJ9lRrkpjqA7Gb30SfIpvIU4X0sF4mdbv4FGc/JcD4qM+DJXE0k+DhmNmPu8" +
            "MItrFyfgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABqfVFxksXFEhjMlMPUrxf1ja7gibof1E49" +
            "vZigAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqXiwCzV/h6uUL2KYwVK8XBOnNTjxbbjrFk" +
            "7EcGdtiUzbBUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qD2GZ+Dnx/yuoM4nlAAN0csYxYXMvDV/e" +
            "u6teeG3c6leQQGAgABNAAAAADwHR8AAAAAAKUAAAAAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX" +
            "7/AKkIBAEFAAcBAQgGBQEJAgMECQeoYQAAAAAAAAoBABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string MintToCheckedMultisigMessage =
            "BAMDCUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyvTw+I1BPNR0VlSo4bmnhoQQ61IkVLEPD0WuhpIewXAM21" +
            "dIJXptqHEGAzyrbL29j+nmPnYitkZQkG68gg4lvRD4ib5rT0bTgxA+F6nChCfZUa5KY6gOxm99EnyKbyFOF9LBeJnW7+BR" +
            "nPyXA+KjPgyVxNJPg4ZjZj7vDCLaxcn4OnC1i7fbauPEwr4QPwO60eHE6R2A3RGXr8HuhWwwwbniwCzV/h6uUL2KYwVK8" +
            "XBOnNTjxbbjrFk7EcGdtiUzbBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F3IgtYUpVZyeIopbd8eq" +
            "6vQpgZ4iEky9O72oNUxQB2XR+CQ9oj6l2DuNeQzPY0Dssm7niyiU8X1dvS0AgcGBAUGAQIDCg6oYQAAAAAAAAoIAQASS" +
            "GVsbG8gZnJvbSBTb2wuTmV0";

        private const string TransferCheckedMultisigMessage =
            "BAMDCUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyD2fbBe7VA8Jg3kfhUQh7HJ6f+hBs2QriyfCGiO1vi" +
            "oqKRK/h3D+lChZA2mVDAGmJHlYiSn8C/yKAGnfXHxgoMvF9c15So4YdnqahN6SHKY5ln1tsHqBpfwwM9RDfRR8GA/" +
            "GByjZ4HWOhY8ZF4ebvMWq3S6h+LX7eLV5BsR18QkUOnC1i7fbauPEwr4QPwO60eHE6R2A3RGXr8HuhWwwwbvSw" +
            "XiZ1u/gUZz8lwPioz4MlcTST4OGY2Y+7wwi2sXJ+6L8pyWJ/BaKAiW8dXpPWFLIy2KbXOugNumqQxVpmiB8G3fbh" +
            "12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqaMCTClqEJuWmr4VslMhwbyIcFZNPtJGGkoxxmHtSOumAQgHBAYF" +
            "BwECAwoMECcAAAAAAAAK";

        private const string BurnCheckedMessage =
            "AQACBUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyDpwtYu322rjxMK+ED8DutHhxOkdgN0Rl6/B7oVsM" +
            "MG70sF4mdbv4FGc/JcD4qM+DJXE0k+DhmNmPu8MItrFyfgbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCp" +
            "BUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qB9EN8DB9phoLwJ2vF3TY8SIjNHH/yA9MfPfvoN5zUesAID" +
            "AwECAAoPqGEAAAAAAAAKBAEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string FreezeAccountMessage =
            "BAMECUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyCvMdxMgG61jhWyn8Fv1ZdwWpUYUJtZIrPCnzv7HW" +
            "fPFTn3mU+LXzjYbXul5/F+k78LFQWQ49hUbwa93RnuSO9GwRoB7PiR430F1c8KIlK9/8p1dvd4bCiUR1JwTbJ5Yy" +
            "P1HIPSemQoFZRkRRtdthf2YQ2HnhQ81DcQftvaA98N21sOi1KvUsX8inslpO8wtEufbBEGIGbN+5YDi5bVWz6nZ/b" +
            "VS3jG60hShrucjgp2V6fcq/E/6fO6aZK5BtOnjBBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F" +
            "3IgtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oBugmJ2LefegA3b6kJPafbq49tUFNOTU5py6T7KOSfF6AgcGBAUGAQI" +
            "DAQoIAQASSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string ThawAccountSetAuthorityMessage =
            "BAMDCUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyCvMdxMgG61jhWyn8Fv1ZdwWpUYUJtZIrPCnzv7HW" +
            "fPFTn3mU+LXzjYbXul5/F+k78LFQWQ49hUbwa93RnuSO9GwRoB7PiR430F1c8KIlK9/8p1dvd4bCiUR1JwTbJ5YyP" +
            "1HIPSemQoFZRkRRtdthf2YQ2HnhQ81DcQftvaA98N21sOi1KvUsX8inslpO8wtEufbBEGIGbN+5YDi5bVWz6nZ/bV" +
            "S3jG60hShrucjgp2V6fcq/E/6fO6aZK5BtOnjBBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F3" +
            "IgtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oF2vvz8uXB7lyB6tJcZj0FSXajkBMaJFtoOucDxiBt2iAwcGBAUGAQID" +
            "AQsHBQUGAQIDIwYCAftRrCs+mafLAQLT3VW772fI0714b6QH4jibZkjI1x/kCAEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string ApproveCheckedMessage =
            "BAMFCkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyhTqOe6WFo5EdWqrNj+11SlkAf/avKcg7AmcS0V1ne1" +
            "50g2o+tFASu728oBWKNjuukzpkMKVbzTVKaY2zOgu8Jbe8MvTWPYLci33hA669YhOTNoYC8GtC7eImCr6c9UnHqh6q" +
            "DWcsER1tQGsj3Y1l8TXqJaoqGvc/lEsGROMo3//wMHwddqmoX0GMOmMiZUpRelZsxO1FdBDNr5QhmEtkGMBs2t64Nb" +
            "n2DhZX+UaG9wt9VT3zkdKONg+Ipqmec3g0IcUCY4xOllbWIWHUtjIqMdsgVFRidruNg7yWWPNg43UG3fbh12Whk9nL" +
            "4UbO63msHLSF7V9bN5E6jPWFfv8AqQVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagdvGLii/m94eoZTnexZ" +
            "xgzw+z6PXaNMoJVckgwRwq588CCAcEBQYHAQIDCg2IEwAAAAAAAAoJAQASSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string ApproveMessage =
            "BAMECUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyIm1L1m3jrd9QYA3DzPHQN1kFLGIJ6vGQA7Ypz1BSv" +
            "i1HtmdFn7ZE8VQa3Cq3XJ1mIUsz4hwJbe7ToemyTsJ9/Jrv9SFWJYlmgS6ev4iDPL7XQ9zIBWmWICKphL+HdgeFTd" +
            "/AW6lmc7MUabf26nqRFd3A6ZpD3XbLyeKVW+pG+MTAbNreuDW59g4WV/lGhvcLfVU985HSjjYPiKapnnN4NP4/SZb" +
            "2oWrgq2s47bqIUQTzMC94kc+nI7GSNjbPW4RaBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F3Ig" +
            "tYUpVZyeIopbd8eq6vQpgZ4iEky9O72oDX06JAMwhe5WoYBGQQmqvqWtrxSzxGc0GxYd2p+X0hWAgcGBAUGAQIDCQ" +
            "SIEwAAAAAAAAgBABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string RevokeMessage =
            "BQQEC0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeywGza3rg1ufYOFlf5Rob3C31VPfOR0o42D4imqZ5z" +
            "eDRYzx0bbqHXZYNhsR3uwPVrTFAMTC0jx72rqhWoIoHmwGnqRA2sKfyYFmprjXtkN3o2rHVwgV4N0RtBqXQmEgro" +
            "Q0bNKgKurzONaQuvuPp40Nj2SwXQm0IkEhFFSRDOwP5RS26fJbuNJr8BprzZZY8EjBVEr0WiG/B7w95r2HrDdGR1" +
            "G8+hzi+pnKdf/z/2NDyrNxlFuCD1zxfJHsIrHKqT8DB8HXapqF9BjDpjImVKUXpWbMTtRXQQza+UIZhLZBgG3fbh" +
            "12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqV4Er7PSNNJpXv/iFEjgEy3WsK0DUo2VDAjvOSz/g9hpBUpTUPhd" +
            "yILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qB+xPGUTXg2vBJsCn9SndzdNl7ce3CFxZEa4Z79/Jic2AMIBAUHBgEK" +
            "DIgTAAAAAAAACggFBQkCAwQBBQoBABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string TransferMultisigMessage =
            "BAMCCEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyD2fbBe7VA8Jg3kfhUQh7HJ6f+hBs2QriyfCGiO1vi" +
            "oqKRK/h3D+lChZA2mVDAGmJHlYiSn8C/yKAGnfXHxgoMvF9c15So4YdnqahN6SHKY5ln1tsHqBpfwwM9RDfRR8GO" +
            "YMO0iFs4aMUVosQrrL+aWspebSXbUiMaf5/Vser1b0OnC1i7fbauPEwr4QPwO60eHE6R2A3RGXr8HuhWwwwbpw3w" +
            "cgNZ/QXTgN9a+S2N3xz3NSvOB6j7IqBlRgLjbjHBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKmB7X+UY" +
            "8KXED5lGuKAmWA0mMQr08QlXyeYC47yCxiX+QEHBgQFBgECAwkDECcAAAAAAAA=";

        private const string BurnMessage =
            "AQACBUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey6Nz9cBhJOumlXLZpUvE8AzAtBfGMn1dZQnsmstBx" +
            "blH+AAGoyvg7ewB86TckKTC3zA8W969k9VFG1UHnZXHxLwbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCp" +
            "BUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qDa9T+ewsQYcaXS8Ka3UtiOeLRbP1wKsuGLfGAyIfGM5QID" +
            "AwECAAkIyAAAAAAAAAAEAQASSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string BurnMultisigMessage =
            "BwYEDUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeycDwgqOcK+3X1trbsFnKJjJKBgrQXdLlTOB5aifN" +
            "HLtKETKhqe0g+wN+JrGfVqRiZwqCoRuq712fzPKETjfAjo37NtrVZKnuugavqUxUkmxtuQXmdFg6sfds4GokwOq" +
            "CnWM8dG26h12WDYbEd7sD1a0xQDEwtI8e9q6oVqCKB5sBp6kQNrCn8mBZqa417ZDd6Nqx1cIFeDdEbQal0JhIK6" +
            "ENGzSoCrq8zjWkLr7j6eNDY9ksF0JtCJBIRRUkQzsD+8DB8HXapqF9BjDpjImVKUXpWbMTtRXQQza+UIZhLZBhR" +
            "S26fJbuNJr8BprzZZY8EjBVEr0WiG/B7w95r2HrDdL36Lc6g35sKjF+do2iqzBqjYUJKXlAnhXDJHsClu85XBt3" +
            "24ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKleBK+z0jTSaV7/4hRI4BMt1rCtA1KNlQwI7zks/4PYaQVKU1" +
            "D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagihio101USobGeALw3d3LcSK6mlzMa9mMnFhtC8Lu0TMDCgYHC" +
            "AkBAgMJBwDKmjsAAAAACgYIBwsEBQYJCCChBwAAAAAADAEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string BurnCheckedMultisigMessage =
            "BwYEDUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeycDwgqOcK+3X1trbsFnKJjJKBgrQXdLlTOB5aifN" +
            "HLtKETKhqe0g+wN+JrGfVqRiZwqCoRuq712fzPKETjfAjo37NtrVZKnuugavqUxUkmxtuQXmdFg6sfds4GokwOq" +
            "CnWM8dG26h12WDYbEd7sD1a0xQDEwtI8e9q6oVqCKB5sBp6kQNrCn8mBZqa417ZDd6Nqx1cIFeDdEbQal0JhIK" +
            "6ENGzSoCrq8zjWkLr7j6eNDY9ksF0JtCJBIRRUkQzsD+8DB8HXapqF9BjDpjImVKUXpWbMTtRXQQza+UIZhLZB" +
            "hRS26fJbuNJr8BprzZZY8EjBVEr0WiG/B7w95r2HrDdL36Lc6g35sKjF+do2iqzBqjYUJKXlAnhXDJHsClu85X" +
            "Bt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKleBK+z0jTSaV7/4hRI4BMt1rCtA1KNlQwI7zks/4PYa" +
            "QVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagcmeXtlaFxMMq4wKOUQQR6lX1/se+NU32JkeqamHOKW" +
            "YDCgYHCAkBAgMKDgDKmjsAAAAACgoGCAcLBAUGCg8goQcAAAAAAAoMAQASSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string CloseAccountMultisigMessage =
            "BAMDCUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyWM8dG26h12WDYbEd7sD1a0xQDEwtI8e9q6oVqC" +
            "KB5sBp6kQNrCn8mBZqa417ZDd6Nqx1cIFeDdEbQal0JhIK6ENGzSoCrq8zjWkLr7j6eNDY9ksF0JtCJBIRRUkQ" +
            "zsD+UUtunyW7jSa/Aaa82WWPBIwVRK9Fohvwe8Pea9h6w3TwMHwddqmoX0GMOmMiZUpRelZsxO1FdBDNr5QhmE" +
            "tkGF4Er7PSNNJpXv/iFEjgEy3WsK0DUo2VDAjvOSz/g9hpBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/" +
            "AKkFSlNQ+F3IgtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oBsCrLfKwExmcW/hntBXRIKAe6vTrQDRoyz2ZvGtaL" +
            "7sAwcGBAUGAQIDCg/gnyZ3AAAAAAoHBgQABgECAwEJCAEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        [TestMethod]
        public void TestTransfer()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(24);
            var newAccount = wallet.GetAccount(26);

            var txInstruction = TokenProgram.Transfer(
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                ownerAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferData, txInstruction.Data);
        }

        [TestMethod]
        public void TestTransferChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(26);
            var newAccount = wallet.GetAccount(27);

            var txInstruction = TokenProgram.TransferChecked(
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                2,
                ownerAccount,
                mintAccount.PublicKey);


            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestTransferCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(26);
            var newAccount = wallet.GetAccount(27);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction = TokenProgram.TransferChecked(
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                2,
                ownerAccount,
                mintAccount.PublicKey,
                signers);


            Assert.AreEqual(9, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey);

            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeMint()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);

            var txInstruction = TokenProgram.InitializeMint(
                mintAccount.PublicKey,
                2,
                ownerAccount.PublicKey,
                ownerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeMintData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeMultisig()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var multiSig = wallet.GetAccount(420);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i).PublicKey);
            }
            var txInstruction = TokenProgram.InitializeMultiSignature(multiSig.PublicKey, signers, 3);

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeMultiSignatureData, txInstruction.Data);
        }

        [TestMethod]
        public void TestMintTo()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToData, txInstruction.Data);
        }

        [TestMethod]
        public void TestMintToChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.MintToChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestMintToCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }
            var txInstruction =
                TokenProgram.MintToChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestBurnChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.BurnChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestBurnCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }
            var txInstruction =
                TokenProgram.BurnChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnCheckedData, txInstruction.Data);
        }


        [TestMethod]
        public void TestApprove()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);

            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey,
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }

        [TestMethod]
        public void TestApproveMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }
            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey,
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }

        [TestMethod]
        public void TestApproveChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.ApproveChecked(
                    sourceAccount.PublicKey,
                    delegateAccount.PublicKey,
                    25000,
                    2,
                    ownerAccount,
                    mintAccount.PublicKey,
                    signers);

            Assert.AreEqual(9, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestApproveCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }
            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey,
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }

        [TestMethod]
        public void TestRevoke()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);

            var txInstruction =
                TokenProgram.Revoke(delegateAccount.PublicKey, ownerAccount);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedRevokeData, txInstruction.Data);
        }

        [TestMethod]
        public void TestRevokeMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }
            var txInstruction =
                TokenProgram.Revoke(delegateAccount.PublicKey, ownerAccount, signers);

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedRevokeData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthorityOwner()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey,
                    AuthorityType.AccountOwner,
                    ownerAccount,
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityOwnerData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthorityOwnerMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey,
                    AuthorityType.AccountOwner,
                    ownerAccount,
                    newOwnerAccount.PublicKey,
                    signers);

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityOwnerData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthorityClose()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey,
                    AuthorityType.CloseAccount,
                    ownerAccount,
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityCloseData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthorityFreeze()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey,
                    AuthorityType.FreezeAccount,
                    ownerAccount,
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityFreezeData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthorityMint()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey,
                    AuthorityType.MintTokens,
                    ownerAccount,
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityMintData, txInstruction.Data);
        }

        [TestMethod]
        public void TestBurn()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);


            var txInstruction =
                TokenProgram.Burn(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey, 25000UL, ownerAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnData, txInstruction.Data);
        }

        [TestMethod]
        public void TestBurnMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.Burn(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey, 25000UL, ownerAccount, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnData, txInstruction.Data);
        }

        [TestMethod]
        public void TestCloseAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.CloseAccount(
                    initialAccount.PublicKey,
                    ownerAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCloseAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestCloseAccountMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.CloseAccount(
                    initialAccount.PublicKey,
                    ownerAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCloseAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestFreezeAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.FreezeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedFreezeAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestFreezeAccountMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.FreezeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedFreezeAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestThawAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var txInstruction =
                TokenProgram.ThawAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedThawAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestThawAccountMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420 + i));
            }

            var txInstruction =
                TokenProgram.ThawAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedThawAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void InitializeMultisigDecodeTest()
        {
            Message msg = Message.Deserialize(InitializeMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(5, decodedInstructions.Count);

            // Create Account instruction
            Assert.AreEqual("Create Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner Account", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Account", out object newAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)newAccount);
            Assert.AreEqual(3361680UL, (ulong)amount);
            Assert.AreEqual(355UL, (ulong)space);

            // initialize multisig instruction
            Assert.AreEqual("Initialize Multisig", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Required Signers", out object numReqSigners));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 4", out object signer4));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 5", out object signer5));
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)account);
            Assert.AreEqual(3, (byte)numReqSigners);
            Assert.AreEqual("DjhLN52wpL6aw9k65MHb3jwxQ7fZ7gfMUGK3gHMBQPWa", (PublicKey)signer1);
            Assert.AreEqual("4h47wFJ7dheVfJJrEcQfx5HvsP3PsfxEqaN38E6pSfhd", (PublicKey)signer2);
            Assert.AreEqual("4gMxwYxoxbSekFNEUtUFfWECF5cp2FRGughfMx22ivwe", (PublicKey)signer3);
            Assert.AreEqual("5BYjVTAYDrRQpMCP4zML3X2v6Jde1sHx3a1bd6DRskVJ", (PublicKey)signer4);
            Assert.AreEqual("AKWjVdBUvekPc2bGf6gKAbQNRSfiXVZ3qFVnP6W8p1W8", (PublicKey)signer5);

            // initialize mint instruction
            Assert.AreEqual("Initialize Mint", decodedInstructions[3].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[3].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[3].PublicKey);
            Assert.AreEqual(0, decodedInstructions[3].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Account", out account));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Mint Authority", out object mintAuthority));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Freeze Authority", out object freezeAuthority));
            Assert.IsTrue(decodedInstructions[3].Values
                .TryGetValue("Freeze Authority Option", out object freezeAuthorityOpt));
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)account);
            Assert.AreEqual(10, (byte)decimals);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)mintAuthority);
            Assert.AreEqual(0, (byte)freezeAuthorityOpt);
            Assert.AreEqual("6eeL1Wb4ufcnxjTtvEStVGHPHeAWexLAFcJ6Kq9pUsXJ", (PublicKey)freezeAuthority);
        }

        [TestMethod]
        public void MintToMultisigDecodeTest()
        {
            Message msg = Message.Deserialize(MintToMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, decodedInstructions.Count);

            // Create Account instruction
            Assert.AreEqual("Create Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner Account", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Account", out object newAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)newAccount);
            Assert.AreEqual(2039280UL, (ulong)amount);
            Assert.AreEqual(165UL, (ulong)space);

            // initialize account instruction
            Assert.AreEqual("Initialize Account", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out owner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)account);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);

            // mint to multisig instruction
            Assert.AreEqual("Mint To", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Mint Authority", out object mintAuthority));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Amount", out amount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Mint", out mint));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual(25000UL, (ulong)amount);
            Assert.AreEqual("DjhLN52wpL6aw9k65MHb3jwxQ7fZ7gfMUGK3gHMBQPWa", (PublicKey)signer1);
            Assert.AreEqual("4h47wFJ7dheVfJJrEcQfx5HvsP3PsfxEqaN38E6pSfhd", (PublicKey)signer2);
            Assert.AreEqual("5BYjVTAYDrRQpMCP4zML3X2v6Jde1sHx3a1bd6DRskVJ", (PublicKey)signer3);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)destination);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)mintAuthority);
        }

        [TestMethod]
        public void DecodeMintToCheckedMessageTest()
        {
            Message msg = Message.Deserialize(MintToCheckedMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Mint To Checked", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint Authority", out object mintAuthority));
            Assert.AreEqual(25000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
            Assert.AreEqual("DjhLN52wpL6aw9k65MHb3jwxQ7fZ7gfMUGK3gHMBQPWa", (PublicKey)signer1);
            Assert.AreEqual("4h47wFJ7dheVfJJrEcQfx5HvsP3PsfxEqaN38E6pSfhd", (PublicKey)signer2);
            Assert.AreEqual("5BYjVTAYDrRQpMCP4zML3X2v6Jde1sHx3a1bd6DRskVJ", (PublicKey)signer3);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)destination);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)mintAuthority);
        }

        [TestMethod]
        public void DecodeTransferCheckedTest()
        {
            Message msg = Message.Deserialize(TransferCheckedMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("Transfer Checked", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual(10000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
            Assert.AreEqual("238y27qL4hnoqByugWhKDA76T7mXHdfK6Qv7fyFqqPYm", (PublicKey)signer1);
            Assert.AreEqual("AJk1YpH1g4A3M4XA1UWbxzMVi6jzSVYBuaRhAkJxK1vH", (PublicKey)signer2);
            Assert.AreEqual("HFgCrTmWC8KGxxSLXN8Xm4FVQU73FtoupdZNDRqfMtV3", (PublicKey)signer3);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)destination);
            Assert.AreEqual("GPpAGnKUSE68JXMcJuws6WVVjTuGH4iqGu5FhbYT3Wk", (PublicKey)source);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("GfYeo1qjCmpRY8nNoeCkNAyXAaecesCmLPorXTekkUKx", (PublicKey)owner);
        }

        [TestMethod]
        public void DecodeBurnCheckedTest()
        {
            Message msg = Message.Deserialize(BurnCheckedMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Burn Checked", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Decimals", out object decimals));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)account);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual(25000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
        }

        [TestMethod]
        public void DecodeFreezeAccountTest()
        {
            Message msg = Message.Deserialize(FreezeAccountMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Freeze Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Freeze Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("5GB1nY6isABT3LzRfbvTYAQpwdSSLoGz88JV1y6PdRLG", (PublicKey)account);
            Assert.AreEqual("DEFFeVTB3ZKUFFncKn2L1jwW4MnLW8UJDkgxpiGRQtaD", (PublicKey)mint);
            Assert.AreEqual("8yZoieywT6CtjK6puZXwg4RASSbQBX3cg9Dnx6LMUgd6", (PublicKey)authority);
            Assert.AreEqual("jk6EcAAv1t4o7Nd4cge3nkXWAmUEcg4HDVvew3szjWp", (PublicKey)signer1);
            Assert.AreEqual("6dRt18mbEHu28fxdyXQGmnLvc9zrp8AsAfBW26vAxVTR", (PublicKey)signer2);
            Assert.AreEqual("8GrcvhyiKdVk9DTYtKkW5qiiR74hevpiQQ1cFMFAmR3o", (PublicKey)signer3);
        }

        [TestMethod]
        public void DecodeThawAccountAndSetAuthorityTest()
        {
            Message msg = Message.Deserialize(ThawAccountSetAuthorityMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);
            Assert.AreEqual("Thaw Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Freeze Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("5GB1nY6isABT3LzRfbvTYAQpwdSSLoGz88JV1y6PdRLG", (PublicKey)account);
            Assert.AreEqual("DEFFeVTB3ZKUFFncKn2L1jwW4MnLW8UJDkgxpiGRQtaD", (PublicKey)mint);
            Assert.AreEqual("8yZoieywT6CtjK6puZXwg4RASSbQBX3cg9Dnx6LMUgd6", (PublicKey)authority);
            Assert.AreEqual("jk6EcAAv1t4o7Nd4cge3nkXWAmUEcg4HDVvew3szjWp", (PublicKey)signer1);
            Assert.AreEqual("6dRt18mbEHu28fxdyXQGmnLvc9zrp8AsAfBW26vAxVTR", (PublicKey)signer2);
            Assert.AreEqual("8GrcvhyiKdVk9DTYtKkW5qiiR74hevpiQQ1cFMFAmR3o", (PublicKey)signer3);

            Assert.AreEqual("Set Authority", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority Type", out object authorityType));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Current Authority", out authority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("New Authority Option", out object authorityOption));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("New Authority", out object newAuthority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out signer3));
            Assert.AreEqual("DEFFeVTB3ZKUFFncKn2L1jwW4MnLW8UJDkgxpiGRQtaD", (PublicKey)account);
            Assert.AreEqual("8yZoieywT6CtjK6puZXwg4RASSbQBX3cg9Dnx6LMUgd6", (PublicKey)authority);
            Assert.AreEqual("Hv3ZhRpKPyLYAwQR2mTPFMYpAQvtwdxxCKKkEVTbrS4j", (PublicKey)newAuthority);
            Assert.AreEqual("jk6EcAAv1t4o7Nd4cge3nkXWAmUEcg4HDVvew3szjWp", (PublicKey)signer1);
            Assert.AreEqual("6dRt18mbEHu28fxdyXQGmnLvc9zrp8AsAfBW26vAxVTR", (PublicKey)signer2);
            Assert.AreEqual("8GrcvhyiKdVk9DTYtKkW5qiiR74hevpiQQ1cFMFAmR3o", (PublicKey)signer3);
            Assert.AreEqual(1, (byte)authorityOption);
            Assert.AreEqual(AuthorityType.AccountOwner, (AuthorityType)authorityType);
        }

        [TestMethod]
        public void DecodeApproveCheckedMultisigTest()
        {
            Message msg = Message.Deserialize(ApproveCheckedMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Approve Checked", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Delegate", out object delegateAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Decimals", out object decimals));
            Assert.AreEqual("CT5RqeDvgrU6NKeGm3f3GBkjAFsVzxusB6cq158nbZJe", (PublicKey)source);
            Assert.AreEqual("HAbjCwXvJLRYPwsfLftT52iPYbHeGPyhPi3QCWiY2CTq", (PublicKey)mint);
            Assert.AreEqual("Dx9YthDvULaC41tJcjJUEMXx7Ky5XQ7jcBx7FdWScCoM", (PublicKey)delegateAccount);
            Assert.AreEqual("3Gph2RRQBBQ9BRPHh6d5DNzfgHSWqM3sP4tdRZPRgWEQ", (PublicKey)authority);
            Assert.AreEqual("9y51foHfw4WzfMR69Tv9hMabAsUeMG8w8AYVpMToVBc9", (PublicKey)signer1);
            Assert.AreEqual("8qpWzMaGsHxwjpv6awy7kqEuCGugGCv4S5BSidDCQ33a", (PublicKey)signer2);
            Assert.AreEqual("DNE44u4kYtiswWUy91eEAypWfZoAhVPjqpQ8JxctY6qC", (PublicKey)signer3);
            Assert.AreEqual(5000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
        }

        [TestMethod]
        public void DecodeApproveMultisigTest()
        {
            Message msg = Message.Deserialize(ApproveMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Approve", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Delegate", out object delegateAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.AreEqual("6EzHDa6PENJVi4iHFPXsThMQpcgQg7QiW4GJj4cyayCX", (PublicKey)source);
            Assert.AreEqual("Dx9YthDvULaC41tJcjJUEMXx7Ky5XQ7jcBx7FdWScCoM", (PublicKey)delegateAccount);
            Assert.AreEqual("J7UXZmSDpp4XbQkwqSxqyRvUb6DJ2SEFhmn3t38KsHu3", (PublicKey)authority);
            Assert.AreEqual("3KPXPJx2U4czq13Dp5E7uw8NutLphpa3EAaUZZuVoxzQ", (PublicKey)signer1);
            Assert.AreEqual("5pwGNb7apN4VBHxXcr9428RDSzyMer3YYcQtLFjMzqX9", (PublicKey)signer2);
            Assert.AreEqual("BRp1JDyCy4xzS77xTjB14mimRhSTBZtaHer5LuueAyTA", (PublicKey)signer3);
            Assert.AreEqual(5000UL, (ulong)amount);
        }

        [TestMethod]
        public void DecodeTransferMultisigTest()
        {
            Message msg = Message.Deserialize(TransferMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("Transfer", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)destination);
            Assert.AreEqual("4sW9XdttQsm1QrfQoRW95jMX4Q5jWYjKkSPEAmkndDUY", (PublicKey)source);
            Assert.AreEqual("BWouvwpvenFxy7Sb2zmDQu1RuWLqFFbK9AbuY8aN96xn", (PublicKey)authority);
            Assert.AreEqual("238y27qL4hnoqByugWhKDA76T7mXHdfK6Qv7fyFqqPYm", (PublicKey)signer1);
            Assert.AreEqual("AJk1YpH1g4A3M4XA1UWbxzMVi6jzSVYBuaRhAkJxK1vH", (PublicKey)signer2);
            Assert.AreEqual("HFgCrTmWC8KGxxSLXN8Xm4FVQU73FtoupdZNDRqfMtV3", (PublicKey)signer3);
            Assert.AreEqual(10000UL, (ulong)amount);
        }

        [TestMethod]
        public void DecodeBurnTest()
        {
            Message msg = Message.Deserialize(BurnMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Burn", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Account", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.AreEqual("Gg12mmahG97PDACxKiBta7ch2kkqDkXUzjn5oAcbPZct", (PublicKey)source);
            Assert.AreEqual("J6WZY5nuYGJmfFtBGZaXgwZSRVuLWxNR6gd4d3XTHqTk", (PublicKey)mint);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)authority);
            Assert.AreEqual(200UL, (ulong)amount);
        }

        [TestMethod]
        public void DecodeBurnMultisigTest()
        {
            Message msg = Message.Deserialize(BurnMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual("Burn", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object source));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("6ULjh7zKSsbiMqNzpCKMgYCUDFZ9Thxy59SGE48x22AF", (PublicKey)source);
            Assert.AreEqual("HAbjCwXvJLRYPwsfLftT52iPYbHeGPyhPi3QCWiY2CTq", (PublicKey)mint);
            Assert.AreEqual("7L1UA4SsaH3AonYX8mHXbLaLR3fiJY71S8zSZQ57WXv8", (PublicKey)authority);
            Assert.AreEqual("6yg3tZM1szHj752RDxQ1GxwvkzR3GyuvAcH498ew1t2T", (PublicKey)signer1);
            Assert.AreEqual("88SzfLipgVTvi8hQwYfq21DgQFcABx6yAwgJH5shfqVZ", (PublicKey)signer2);
            Assert.AreEqual("5Xcw7EQb6msgpVdGB8Hf8kpCqVyacTChgFBUphpuUeBo", (PublicKey)signer3);
            Assert.AreEqual(500000UL, (ulong)amount);
        }

        [TestMethod]
        public void DecodeBurnCheckedMultisigTest()
        {
            Message msg = Message.Deserialize(BurnCheckedMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual("Burn Checked", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object source));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("6ULjh7zKSsbiMqNzpCKMgYCUDFZ9Thxy59SGE48x22AF", (PublicKey)source);
            Assert.AreEqual("HAbjCwXvJLRYPwsfLftT52iPYbHeGPyhPi3QCWiY2CTq", (PublicKey)mint);
            Assert.AreEqual("7L1UA4SsaH3AonYX8mHXbLaLR3fiJY71S8zSZQ57WXv8", (PublicKey)authority);
            Assert.AreEqual("6yg3tZM1szHj752RDxQ1GxwvkzR3GyuvAcH498ew1t2T", (PublicKey)signer1);
            Assert.AreEqual("88SzfLipgVTvi8hQwYfq21DgQFcABx6yAwgJH5shfqVZ", (PublicKey)signer2);
            Assert.AreEqual("5Xcw7EQb6msgpVdGB8Hf8kpCqVyacTChgFBUphpuUeBo", (PublicKey)signer3);
            Assert.AreEqual(500000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
        }

        [TestMethod]
        public void DecodeRevokeMultisigTest()
        {
            Message msg = Message.Deserialize(RevokeMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);
            Assert.AreEqual("Revoke", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("6ULjh7zKSsbiMqNzpCKMgYCUDFZ9Thxy59SGE48x22AF", (PublicKey)source);
            Assert.AreEqual("7L1UA4SsaH3AonYX8mHXbLaLR3fiJY71S8zSZQ57WXv8", (PublicKey)authority);
            Assert.AreEqual("6yg3tZM1szHj752RDxQ1GxwvkzR3GyuvAcH498ew1t2T", (PublicKey)signer1);
            Assert.AreEqual("88SzfLipgVTvi8hQwYfq21DgQFcABx6yAwgJH5shfqVZ", (PublicKey)signer2);
            Assert.AreEqual("5Xcw7EQb6msgpVdGB8Hf8kpCqVyacTChgFBUphpuUeBo", (PublicKey)signer3);
        }

        [TestMethod]
        public void DecodeBurnCheckedAndCloseMultisigTest()
        {
            Message msg = Message.Deserialize(CloseAccountMultisigMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual("Burn Checked", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Account", out object source));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual("6ULjh7zKSsbiMqNzpCKMgYCUDFZ9Thxy59SGE48x22AF", (PublicKey)source);
            Assert.AreEqual("HAbjCwXvJLRYPwsfLftT52iPYbHeGPyhPi3QCWiY2CTq", (PublicKey)mint);
            Assert.AreEqual("7L1UA4SsaH3AonYX8mHXbLaLR3fiJY71S8zSZQ57WXv8", (PublicKey)authority);
            Assert.AreEqual("6yg3tZM1szHj752RDxQ1GxwvkzR3GyuvAcH498ew1t2T", (PublicKey)signer1);
            Assert.AreEqual("88SzfLipgVTvi8hQwYfq21DgQFcABx6yAwgJH5shfqVZ", (PublicKey)signer2);
            Assert.AreEqual("5Xcw7EQb6msgpVdGB8Hf8kpCqVyacTChgFBUphpuUeBo", (PublicKey)signer3);
            Assert.AreEqual(1999020000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);

            Assert.AreEqual("Close Account", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out source));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out authority));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out signer3));
            Assert.AreEqual("6ULjh7zKSsbiMqNzpCKMgYCUDFZ9Thxy59SGE48x22AF", (PublicKey)source);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)destination);
            Assert.AreEqual("6yg3tZM1szHj752RDxQ1GxwvkzR3GyuvAcH498ew1t2T", (PublicKey)signer1);
            Assert.AreEqual("88SzfLipgVTvi8hQwYfq21DgQFcABx6yAwgJH5shfqVZ", (PublicKey)signer2);
            Assert.AreEqual("5Xcw7EQb6msgpVdGB8Hf8kpCqVyacTChgFBUphpuUeBo", (PublicKey)signer3);
        }
    }
}