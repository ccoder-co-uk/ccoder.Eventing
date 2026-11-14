// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public sealed partial class HttpEventQueueServiceTests
{
    [Fact]
    public void ShouldClassifyQueueFoundationFailures()
    {
        // Given

        Mock<IHttpEventBroker> invalidBroker = new();

        invalidBroker
            .Setup(expression: value => value.GetQueueSnapshots())
            .Throws<ArgumentException>();

        Mock<IHttpEventBroker> dependencyBroker = new();

        dependencyBroker
            .Setup(expression: value => value.GetQueueSnapshots())
            .Throws<InvalidOperationException>();

        Mock<IHttpEventBroker> unexpectedBroker = new();

        unexpectedBroker
            .Setup(expression: value => value.GetQueueSnapshots())
            .Throws<NotSupportedException>();

        HttpEventQueueService invalidService = new(
            httpEventBroker: invalidBroker.Object);

        HttpEventQueueService dependencyService = new(
            httpEventBroker: dependencyBroker.Object);

        HttpEventQueueService unexpectedService = new(
            httpEventBroker: unexpectedBroker.Object);

        // When

        Action invalid = () => invalidService.GetQueueReport();
        Action dependency = () => dependencyService.GetQueueReport();
        Action unexpected = () => unexpectedService.GetQueueReport();

        // Then

        Assert.Throws<ServiceValidationException>(testCode: invalid);
        Assert.Throws<ServiceDependencyException>(testCode: dependency);
        Assert.Throws<ServiceException>(testCode: unexpected);
    }

    [Fact]
    public void ShouldClassifyQueueProcessingFailures()
    {
        // Given

        Mock<IHttpEventQueueService> invalidFoundation = new();

        invalidFoundation
            .Setup(expression: value => value.GetQueueReport())
            .Throws<ArgumentException>();

        Mock<IHttpEventQueueService> dependencyFoundation = new();

        dependencyFoundation
            .Setup(expression: value => value.GetQueueReport())
            .Throws<InvalidOperationException>();

        Mock<IHttpEventQueueService> unexpectedFoundation = new();

        unexpectedFoundation
            .Setup(expression: value => value.GetQueueReport())
            .Throws<NotSupportedException>();

        HttpEventQueueProcessingService invalidService = new(
            queueService: invalidFoundation.Object);

        HttpEventQueueProcessingService dependencyService = new(
            queueService: dependencyFoundation.Object);

        HttpEventQueueProcessingService unexpectedService = new(
            queueService: unexpectedFoundation.Object);

        // When

        Action invalid = () => invalidService.GetQueueReport();
        Action dependency = () => dependencyService.GetQueueReport();
        Action unexpected = () => unexpectedService.GetQueueReport();

        // Then

        Assert.Throws<ServiceValidationException>(testCode: invalid);
        Assert.Throws<ServiceDependencyException>(testCode: dependency);
        Assert.Throws<ServiceException>(testCode: unexpected);
    }
}