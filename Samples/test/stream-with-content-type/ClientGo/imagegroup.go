package

// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

import (
    "github.com/Azure/go-autorest/autorest"
    "github.com/Azure/go-autorest/autorest/azure"
    "net/http"
    "io"
)

// ImageGroupClient is the client for the ImageGroup methods of the  service.
type ImageGroupClient struct {
    ManagementClient
}
// NewImageGroupClient creates an instance of the ImageGroupClient client.
func NewImageGroupClient() ImageGroupClient {
        return NewImageGroupClientWithBaseURI(DefaultBaseURI, )
        }

// NewImageGroupClientWithBaseURI creates an instance of the ImageGroupClient client.
    func NewImageGroupClientWithBaseURI(baseURI string, ) ImageGroupClient {
        return ImageGroupClient{ NewWithBaseURI(baseURI, )}
    }

// AMethod sends the a method request.
//
// imageParameter is an image stream. imageParameter will be closed upon successful return. Callers should ensure
// closure when receiving an error.imageContentType is the content type of the image.
func (client ImageGroupClient) AMethod(imageParameter io.ReadCloser, imageContentType ImageTypeEnum) (result autorest.Response, err error) {
    req, err := client.AMethodPreparer(imageParameter, imageContentType)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "AMethod", nil , "Failure preparing request")
        return
    }

    resp, err := client.AMethodSender(req)
    if err != nil {
        result.Response = resp
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "AMethod", resp, "Failure sending request")
        return
    }

    result, err = client.AMethodResponder(resp)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "AMethod", resp, "Failure responding to request")
    }

    return
}

// AMethodPreparer prepares the AMethod request.
func (client ImageGroupClient) AMethodPreparer(imageParameter io.ReadCloser, imageContentType ImageTypeEnum) (*http.Request, error) {
    preparer := autorest.CreatePreparer(
                        autorest.AsPost(),
                        autorest.WithBaseURL(client.BaseURI),
                        autorest.WithPath("/ProcessImage/FunctionA"),
                        autorest.WithFile(imageParameter),
                        autorest.WithHeader("Content-Type",autorest.String(imageContentType)))
    return preparer.Prepare(&http.Request{})
}

// AMethodSender sends the AMethod request. The method will close the
// http.Response Body if it receives an error.
func (client ImageGroupClient) AMethodSender(req *http.Request) (*http.Response, error) {
    return autorest.SendWithSender(client, req)
}

// AMethodResponder handles the response to the AMethod request. The method always
// closes the http.Response Body.
func (client ImageGroupClient) AMethodResponder(resp *http.Response) (result autorest.Response, err error) {
    err = autorest.Respond(
            resp,
            client.ByInspecting(),
            azure.WithErrorUnlessStatusCode(http.StatusOK),
            autorest.ByClosing())
    result.Response = resp
    return
}

// BMethod sends the b method request.
//
// imageParameter is an image stream. imageParameter will be closed upon successful return. Callers should ensure
// closure when receiving an error.imageContentType is the content type of the image.
func (client ImageGroupClient) BMethod(imageParameter io.ReadCloser, imageContentType ImageTypeEnum) (result autorest.Response, err error) {
    req, err := client.BMethodPreparer(imageParameter, imageContentType)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "BMethod", nil , "Failure preparing request")
        return
    }

    resp, err := client.BMethodSender(req)
    if err != nil {
        result.Response = resp
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "BMethod", resp, "Failure sending request")
        return
    }

    result, err = client.BMethodResponder(resp)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "BMethod", resp, "Failure responding to request")
    }

    return
}

// BMethodPreparer prepares the BMethod request.
func (client ImageGroupClient) BMethodPreparer(imageParameter io.ReadCloser, imageContentType ImageTypeEnum) (*http.Request, error) {
    preparer := autorest.CreatePreparer(
                        autorest.AsPost(),
                        autorest.WithBaseURL(client.BaseURI),
                        autorest.WithPath("/ProcessImage/FunctionB"),
                        autorest.WithFile(imageParameter),
                        autorest.WithHeader("Content-Type",autorest.String(imageContentType)))
    return preparer.Prepare(&http.Request{})
}

// BMethodSender sends the BMethod request. The method will close the
// http.Response Body if it receives an error.
func (client ImageGroupClient) BMethodSender(req *http.Request) (*http.Response, error) {
    return autorest.SendWithSender(client, req)
}

// BMethodResponder handles the response to the BMethod request. The method always
// closes the http.Response Body.
func (client ImageGroupClient) BMethodResponder(resp *http.Response) (result autorest.Response, err error) {
    err = autorest.Respond(
            resp,
            client.ByInspecting(),
            azure.WithErrorUnlessStatusCode(http.StatusOK),
            autorest.ByClosing())
    result.Response = resp
    return
}

// CMethod sends the c method request.
//
// imageParameter is an image stream. imageParameter will be closed upon successful return. Callers should ensure
// closure when receiving an error.imageContentType is the content type of the image.
func (client ImageGroupClient) CMethod(imageParameter io.ReadCloser, imageContentType ImageTypeRestrictedEnum) (result autorest.Response, err error) {
    req, err := client.CMethodPreparer(imageParameter, imageContentType)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "CMethod", nil , "Failure preparing request")
        return
    }

    resp, err := client.CMethodSender(req)
    if err != nil {
        result.Response = resp
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "CMethod", resp, "Failure sending request")
        return
    }

    result, err = client.CMethodResponder(resp)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "CMethod", resp, "Failure responding to request")
    }

    return
}

// CMethodPreparer prepares the CMethod request.
func (client ImageGroupClient) CMethodPreparer(imageParameter io.ReadCloser, imageContentType ImageTypeRestrictedEnum) (*http.Request, error) {
    preparer := autorest.CreatePreparer(
                        autorest.AsPost(),
                        autorest.WithBaseURL(client.BaseURI),
                        autorest.WithPath("/ProcessImage/FunctionC"),
                        autorest.WithFile(imageParameter),
                        autorest.WithHeader("Content-Type",autorest.String(imageContentType)))
    return preparer.Prepare(&http.Request{})
}

// CMethodSender sends the CMethod request. The method will close the
// http.Response Body if it receives an error.
func (client ImageGroupClient) CMethodSender(req *http.Request) (*http.Response, error) {
    return autorest.SendWithSender(client, req)
}

// CMethodResponder handles the response to the CMethod request. The method always
// closes the http.Response Body.
func (client ImageGroupClient) CMethodResponder(resp *http.Response) (result autorest.Response, err error) {
    err = autorest.Respond(
            resp,
            client.ByInspecting(),
            azure.WithErrorUnlessStatusCode(http.StatusOK),
            autorest.ByClosing())
    result.Response = resp
    return
}

// DMethod sends the d method request.
//
// imageParameter is an image stream. imageParameter will be closed upon successful return. Callers should ensure
// closure when receiving an error.imageContentType is the content type of the image.
func (client ImageGroupClient) DMethod(imageParameter io.ReadCloser, imageContentType ImageTypeRestrictedStringsEnum) (result autorest.Response, err error) {
    req, err := client.DMethodPreparer(imageParameter, imageContentType)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "DMethod", nil , "Failure preparing request")
        return
    }

    resp, err := client.DMethodSender(req)
    if err != nil {
        result.Response = resp
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "DMethod", resp, "Failure sending request")
        return
    }

    result, err = client.DMethodResponder(resp)
    if err != nil {
        err = autorest.NewErrorWithError(err, ".ImageGroupClient", "DMethod", resp, "Failure responding to request")
    }

    return
}

// DMethodPreparer prepares the DMethod request.
func (client ImageGroupClient) DMethodPreparer(imageParameter io.ReadCloser, imageContentType ImageTypeRestrictedStringsEnum) (*http.Request, error) {
    preparer := autorest.CreatePreparer(
                        autorest.AsPost(),
                        autorest.WithBaseURL(client.BaseURI),
                        autorest.WithPath("/ProcessImage/FunctionD"),
                        autorest.WithFile(imageParameter),
                        autorest.WithHeader("Content-Type",autorest.String(imageContentType)))
    return preparer.Prepare(&http.Request{})
}

// DMethodSender sends the DMethod request. The method will close the
// http.Response Body if it receives an error.
func (client ImageGroupClient) DMethodSender(req *http.Request) (*http.Response, error) {
    return autorest.SendWithSender(client, req)
}

// DMethodResponder handles the response to the DMethod request. The method always
// closes the http.Response Body.
func (client ImageGroupClient) DMethodResponder(resp *http.Response) (result autorest.Response, err error) {
    err = autorest.Respond(
            resp,
            client.ByInspecting(),
            azure.WithErrorUnlessStatusCode(http.StatusOK),
            autorest.ByClosing())
    result.Response = resp
    return
}

