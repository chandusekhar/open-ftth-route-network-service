﻿using CSharpFunctionalExtensions;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Mutations;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.RouteNetwork.Tests.Fixtures;
using System;
using Xunit;

namespace OpenFTTH.RouteNetwork.Tests
{
    public class InterestMutationTests : IClassFixture<TestRouteNetwork>
    {
        readonly TestRouteNetwork testNetwork;

        public InterestMutationTests(TestRouteNetwork testNetwork)
        {
            this.testNetwork = testNetwork;
        }

        [Fact]
        public async void CreateValidWalkOfInterestUsingOneSegmentIdOnly_ShouldReturnSuccess()
        {
            // Route network subset used in this test:
            // (CO_1) <- (S1) -> (HH_1)
            var interestId = Guid.NewGuid();

            var walk = new RouteNetworkElementIdList() { testNetwork.S1 };

            // Act
            var registerWalkOfInterestCommand = new RegisterWalkOfInterestCommand(interestId, walk);

            Result<RegisterWalkOfInterestCommandResult> registerWalkOfInterestCommandResult = await testNetwork.CommandApi.HandleAsync(registerWalkOfInterestCommand);

            // Assert command success and that the command result include all three route network element ids
            Assert.True(registerWalkOfInterestCommandResult.IsSuccess);
            Assert.Equal(3, registerWalkOfInterestCommandResult.Value.Walk.Count);
            Assert.Equal(testNetwork.CO_1, registerWalkOfInterestCommandResult.Value.Walk[0]);
            Assert.Equal(testNetwork.S1, registerWalkOfInterestCommandResult.Value.Walk[1]);
            Assert.Equal(testNetwork.HH_1, registerWalkOfInterestCommandResult.Value.Walk[2]);
        }

        [Fact]
        public async void CreateValidWalkOfInterestUsingThreeSegments_ShouldReturnSuccess()
        {
            // Route network subset used in this test:
            // (CO_1) <- (S1) -> (HH_1) <- (S2) -> (HH_2) <- (S4) -> (CC_1)
            var interestId = Guid.NewGuid();

            var walk = new RouteNetworkElementIdList() { testNetwork.S1, testNetwork.S2, testNetwork.S4 };

            var createInterestCommand = new RegisterWalkOfInterestCommand(interestId, walk);

            // Act
            Result<RegisterWalkOfInterestCommandResult> registerWalkOfInterestCommandResult = await testNetwork.CommandApi.HandleAsync(createInterestCommand);

            // Assert command success and that the result include all three route network element ids
            Assert.True(registerWalkOfInterestCommandResult.IsSuccess);
            Assert.Equal(7, registerWalkOfInterestCommandResult.Value.Walk.Count);
            Assert.Equal(testNetwork.CO_1, registerWalkOfInterestCommandResult.Value.Walk[0]);
            Assert.Equal(testNetwork.S1, registerWalkOfInterestCommandResult.Value.Walk[1]);
            Assert.Equal(testNetwork.HH_1, registerWalkOfInterestCommandResult.Value.Walk[2]);
            Assert.Equal(testNetwork.S2, registerWalkOfInterestCommandResult.Value.Walk[3]);
            Assert.Equal(testNetwork.HH_2, registerWalkOfInterestCommandResult.Value.Walk[4]);
            Assert.Equal(testNetwork.S4, registerWalkOfInterestCommandResult.Value.Walk[5]);
            Assert.Equal(testNetwork.CC_1, registerWalkOfInterestCommandResult.Value.Walk[6]);
        }

        [Fact]
        public async void CreateValidWalkOfInterestOverlappingSegments_ShouldReturnSuccess()
        {
            // Route network subset used in this test:
            // (CO_1) <- (S1) -> (HH_1) <- (S2) -> (HH_2) <- (S4) -> (CC_1)
            var interestId = Guid.NewGuid();

            var walk = new RouteNetworkElementIdList() { testNetwork.S1, testNetwork.S2, testNetwork.S1 };

            var createInterestCommand = new RegisterWalkOfInterestCommand(interestId, walk);

            // Act
            Result<RegisterWalkOfInterestCommandResult> registerWalkOfInterestCommandResult = await testNetwork.CommandApi.HandleAsync(createInterestCommand);

            // Assert command success and that the result include all three route network element ids
            Assert.True(registerWalkOfInterestCommandResult.IsSuccess);
            Assert.Equal(7, registerWalkOfInterestCommandResult.Value.Walk.Count);
            Assert.Equal(testNetwork.CO_1, registerWalkOfInterestCommandResult.Value.Walk[0]);
            Assert.Equal(testNetwork.S1, registerWalkOfInterestCommandResult.Value.Walk[1]);
            Assert.Equal(testNetwork.HH_1, registerWalkOfInterestCommandResult.Value.Walk[2]);
            Assert.Equal(testNetwork.S2, registerWalkOfInterestCommandResult.Value.Walk[3]);
            Assert.Equal(testNetwork.HH_2, registerWalkOfInterestCommandResult.Value.Walk[4]);
            Assert.Equal(testNetwork.S1, registerWalkOfInterestCommandResult.Value.Walk[5]);
            Assert.Equal(testNetwork.CO_1, registerWalkOfInterestCommandResult.Value.Walk[6]);
        }

        [Fact]
        public async void CreateInvalidWalkOfInterestUsingOneNodeAndOneSegments_ShouldReturnFaliour()
        {
            // Route network subset used in this test:
            // (CO_1) <- (S1)
            var interestId = Guid.NewGuid();

            var walk = new RouteNetworkElementIdList() { testNetwork.CO_1, testNetwork.S1 };

            var createInterestCommand = new RegisterWalkOfInterestCommand(interestId, walk);

            // Act
            Result<RegisterWalkOfInterestCommandResult> registerWalkOfInterestCommandResult = await testNetwork.CommandApi.HandleAsync(createInterestCommand);

            // Assert
            Assert.True(registerWalkOfInterestCommandResult.IsFailure);
        }

        [Fact]
        public async void CreateInvalidWalkOfInterestUsingTwoSeparatedSegments_ShouldReturnFaliour()
        {
            // Route network subset used in this test:
            // (CO_1) <- (S1) -> (HH_1) hole in the walk here (HH_2) -> (S4) -> (CC_1)
            var interestId = Guid.NewGuid();

            var walk = new RouteNetworkElementIdList() { testNetwork.S1, testNetwork.S4 };

            var createInterestCommand = new RegisterWalkOfInterestCommand(interestId, walk);

            // Act
            Result<RegisterWalkOfInterestCommandResult> registerWalkOfInterestCommandResult = await testNetwork.CommandApi.HandleAsync(createInterestCommand);

            // Assert
            Assert.True(registerWalkOfInterestCommandResult.IsFailure);
        }



    }
}
