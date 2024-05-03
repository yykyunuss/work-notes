package com.ykb.factoring.factoring.cheque.bff.client;

import com.ykb.factoring.factoring.cheque.bff.config.CustomerClientFallback;
import com.ykb.factoring.factoring.cheque.bff.config.FeignAuthorizationConfig;
import com.ykb.factoring.factoring.cheque.bff.dto.customer.ResponseCustomerInformation;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;

@FeignClient(
        contextId = "customerClient",
        name = "${be.customer.name}",
        url = "${be.customer.url:}",
        path = "${be.customer.path:}",
        fallback = CustomerClientFallback.class,
        configuration = FeignAuthorizationConfig.class
)
public interface CustomerInformationClient {
    @GetMapping(value = "/customer/{customerId}/customer-information", produces = MediaType.APPLICATION_JSON_VALUE)
    ResponseCustomerInformation findCustomerInformation(@PathVariable("customerId") Long customerId);
}
