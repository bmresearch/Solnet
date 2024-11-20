using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.Test;


[TestClass]
public class BpfLoaderProgramTest
{
    private string _expectedBorshStringResult =
        "0B00000068656C6C6F20776F726C6400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
    private string _expectedBorshStringResult2 =
        "000B00000068656C6C6F20776F726C64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
    [TestMethod]
    public  void BorshStringSerializationTest()
    {
        var sampleString = "hello world";
        var buffer = new byte[100];
        buffer.WriteBorshString( sampleString, 0);
        Assert.AreEqual( Convert.ToHexString(buffer) ,_expectedBorshStringResult );
        buffer = new byte[100];
        buffer.WriteBorshString( sampleString, 1);
        Assert.AreEqual( Convert.ToHexString(buffer) ,_expectedBorshStringResult2 );
    }

    [TestMethod]
    public void BorshStringDeserializationTest()
    {
        var sampleString = "hello world";
        var sample1 =  new ReadOnlySpan<byte>(Convert.FromHexString(_expectedBorshStringResult)) ;
        sample1.GetBorshString(0, out var result);
        Assert.AreEqual(sampleString,result );
        sample1 =  new ReadOnlySpan<byte>(Convert.FromHexString(_expectedBorshStringResult2)) ;
        sample1.GetBorshString(1, out var result2);
        Assert.AreEqual(sampleString,result2 );
    }
}