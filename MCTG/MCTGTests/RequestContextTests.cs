using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCTG;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG.Tests
{
    [TestClass()]
    public class RequestContextTests
    {
        [TestMethod()]
        public void receiveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void POSTTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GETTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PUTTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DELTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ListAllMsgTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateHttpResponseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetHeaderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBodyTest()
        {
            //Arrange
            string req = "curl -X POST http://localhost:10001/users --header \"Content - Type: application / json\" -d \"{\"Username\":\"admin\",    \"Password\":\"istrator\"}";
            string[] line;
            string body = "";
            line = req.Split("\r\n");
            string[] first_line = line[0].Split(' ');
            string[] res = first_line[1].Split('/');

            //Act
            int i = 0;
            int hv = 0;
            while (i <= line.Length)
            {
                if (line[i] == "")
                {
                    if (hv == 0)
                    {
                        body += line[i + 1];
                    }
                    hv = 1;
                }
                i++;
            }
            //Assert
            Assert.AreEqual("\"{\"Username\":\"admin\",    \"Password\":\"istrator\"}",body);
        }

        [TestMethod()]
        public void FileNotFoundTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DirNotFoundTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DontUseFileIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NoFileIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NoContentTest()
        {
            Assert.Fail();
        }
    }
}