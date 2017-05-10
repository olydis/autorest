/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator.
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

package fixtures.bodycomplex;

import com.microsoft.rest.ServiceCallback;
import com.microsoft.rest.ServiceFuture;
import com.microsoft.rest.ServiceResponse;
import fixtures.bodycomplex.models.ErrorException;
import fixtures.bodycomplex.models.Fish;
import java.io.IOException;
import rx.Observable;

/**
 * An instance of this class provides access to all the operations defined
 * in Polymorphicrecursives.
 */
public interface Polymorphicrecursives {
    /**
     * Get complex types that are polymorphic and have recursive references.
     *
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @throws ErrorException thrown if the request is rejected by server
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent
     * @return the Fish object if successful.
     */
    Fish getValid();

    /**
     * Get complex types that are polymorphic and have recursive references.
     *
     * @param serviceCallback the async ServiceCallback to handle successful and failed responses.
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the {@link ServiceFuture} object
     */
    ServiceFuture<Fish> getValidAsync(final ServiceCallback<Fish> serviceCallback);

    /**
     * Get complex types that are polymorphic and have recursive references.
     *
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the observable to the Fish object
     */
    Observable<Fish> getValidAsync();

    /**
     * Get complex types that are polymorphic and have recursive references.
     *
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the observable to the Fish object
     */
    Observable<ServiceResponse<Fish>> getValidWithServiceResponseAsync();

    /**
     * Put complex types that are polymorphic and have recursive references.
     *
     * @param complexBody Please put a salmon that looks like this: { "fishtype": "salmon", "species": "king", "length": 1, "age": 1, "location": "alaska", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6, "siblings": [ { "fishtype": "salmon", "species": "coho", "length": 2, "age": 2, "location": "atlantic", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6 }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @throws ErrorException thrown if the request is rejected by server
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent
     */
    void putValid(Fish complexBody);

    /**
     * Put complex types that are polymorphic and have recursive references.
     *
     * @param complexBody Please put a salmon that looks like this: { "fishtype": "salmon", "species": "king", "length": 1, "age": 1, "location": "alaska", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6, "siblings": [ { "fishtype": "salmon", "species": "coho", "length": 2, "age": 2, "location": "atlantic", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6 }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }
     * @param serviceCallback the async ServiceCallback to handle successful and failed responses.
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the {@link ServiceFuture} object
     */
    ServiceFuture<Void> putValidAsync(Fish complexBody, final ServiceCallback<Void> serviceCallback);

    /**
     * Put complex types that are polymorphic and have recursive references.
     *
     * @param complexBody Please put a salmon that looks like this: { "fishtype": "salmon", "species": "king", "length": 1, "age": 1, "location": "alaska", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6, "siblings": [ { "fishtype": "salmon", "species": "coho", "length": 2, "age": 2, "location": "atlantic", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6 }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the {@link ServiceResponse} object if successful.
     */
    Observable<Void> putValidAsync(Fish complexBody);

    /**
     * Put complex types that are polymorphic and have recursive references.
     *
     * @param complexBody Please put a salmon that looks like this: { "fishtype": "salmon", "species": "king", "length": 1, "age": 1, "location": "alaska", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6, "siblings": [ { "fishtype": "salmon", "species": "coho", "length": 2, "age": 2, "location": "atlantic", "iswild": true, "siblings": [ { "fishtype": "shark", "species": "predator", "length": 20, "age": 6 }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }, { "fishtype": "sawshark", "species": "dangerous", "length": 10, "age": 105 } ] }
     * @throws IllegalArgumentException thrown if parameters fail the validation
     * @return the {@link ServiceResponse} object if successful.
     */
    Observable<ServiceResponse<Void>> putValidWithServiceResponseAsync(Fish complexBody);

}
