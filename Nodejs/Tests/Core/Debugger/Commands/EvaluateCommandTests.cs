﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using Microsoft.NodejsTools.Debugger;
using Microsoft.NodejsTools.Debugger.Commands;
using Microsoft.NodejsTools.Debugger.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NodejsTests.Debugger.Commands {
    [TestClass]
    public class EvaluateCommandTests {
        [TestMethod]
        public void CreateEvaluateCommand() {
            // Arrange
            const int commandId = 3;
            var resultFactoryMock = new Mock<IEvaluationResultFactory>();
            const string expression = "expression";

            // Act
            var evaluateCommand = new EvaluateCommand(commandId, resultFactoryMock.Object, expression);

            // Assert
            Assert.AreEqual(commandId, evaluateCommand.Id);
            Assert.AreEqual(
                string.Format(
                    "{{\"command\":\"evaluate\",\"seq\":{0},\"type\":\"request\",\"arguments\":{{\"expression\":\"{1}\",\"frame\":0,\"global\":false,\"disable_break\":true,\"maxStringLength\":-1}}}}",
                    commandId, expression),
                evaluateCommand.ToString());
        }

        [TestMethod]
        public void CreateEvaluateCommandWithVariableId() {
            // Arrange
            const int commandId = 3;
            var resultFactoryMock = new Mock<IEvaluationResultFactory>();
            const int variableId = 2;

            // Act
            var evaluateCommand = new EvaluateCommand(commandId, resultFactoryMock.Object, variableId);

            // Assert
            Assert.AreEqual(commandId, evaluateCommand.Id);
            Assert.AreEqual(
                string.Format(
                    "{{\"command\":\"evaluate\",\"seq\":{0},\"type\":\"request\",\"arguments\":{{\"expression\":\"variable.toString()\",\"frame\":0,\"global\":false,\"disable_break\":true,\"additional_context\":[{{\"name\":\"variable\",\"handle\":{1}}}],\"maxStringLength\":-1}}}}",
                    commandId, variableId),
                evaluateCommand.ToString());
        }

        [TestMethod]
        public void ProcessEvaluateResponse() {
            // Arrange
            const int commandId = 3;
            var resultFactoryMock = new Mock<IEvaluationResultFactory>();
            resultFactoryMock.Setup(factory => factory.Create(It.IsAny<INodeVariable>()))
                .Returns(() => new NodeEvaluationResult(0, null, null, null, null, null, NodeExpressionType.None, null));
            const string expression = "expression";
            var stackFrame = new NodeStackFrame(null, null, null, 0, 0, 0, 0);
            var evaluateCommand = new EvaluateCommand(commandId, resultFactoryMock.Object, expression, stackFrame);

            // Act
            evaluateCommand.ProcessResponse(SerializationTestData.GetEvaluateResponse());

            // Assert
            Assert.AreEqual(commandId, evaluateCommand.Id);
            Assert.IsNotNull(evaluateCommand.Result);
            resultFactoryMock.Verify(factory => factory.Create(It.IsAny<INodeVariable>()), Times.Once);
        }
    }
}