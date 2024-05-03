import React, { useRef, useState } from "react";
import {
  Button,
  Form,
  Select,
  TextInput,
  styled,
  DatePicker,
  Message,
} from "ykb-ui";
import { oneOfEightSpan, quarterSpan } from "../ChequeQuery/utils";
import FactorCustomerSearch from "fctr-customer-search";

import {
  useIntl,
  usePermission,
  useShellCommunicator,
  withPermission,
} from "factor-shell";
import {
  setChequeInfos,
  setCustomerList,
  setDrawerList,
  setIntermediateEndorserList,
  setLastEndorserList,
  setTableLoading,
} from "../../store/chequeQuerySlice";
import { useDispatch, useSelector } from "react-redux";
import api from "../../api";

export const StyledContent = styled.div`
  .ghost {
    visibility: hidden;
  }
`;

const moment = require("moment");
const { RangePicker } = DatePicker;

const now = moment();
now.locale("tr");

const ChequeQueryFilter = () => {
  const { i18n } = useIntl();

  const formRefCustomer = useRef();
  const formRefChequeInfo = useRef();
  const formRefDates = useRef();

  const dispatch = useDispatch();

  const {
    customerList,
    drawerList,
    lastEndorserList,
    intermediateEndorserList,
    currencies,
  } = useSelector(({ chequeQuery }) => chequeQuery);

  const [customer, setCustomer] = useState({});

  //const [customerNameSurname, setCustomerNameSurname] = useState({});

  const getRoleTypesAndCustomerNos = () => {
    const roleTypes = formRefCustomer.current.getFieldsValue();

    const chequeRoles = [];
    if (roleTypes.drawer !== undefined) {
      let chequeRole = {};
      let roleType = "DRAWER";
      if (Array.isArray(roleTypes.drawer)) {
        for (const element of roleTypes.drawer) {
          const customerId = element.substring(0, element.indexOf("-"));
          chequeRole = { customerId, roleType };
          chequeRoles.push(chequeRole);
        }
      } else {
        const customerId = roleTypes.drawer.substring(
          0,
          roleTypes.drawer.indexOf("-")
        );
        chequeRole = { customerId, roleType };
        chequeRoles.push(chequeRole);
      }
    }

    if (roleTypes.intermediateEndorser !== undefined) {
      let chequeRole = {};
      let roleType = "ENDORSER";
      if (Array.isArray(roleTypes.intermediateEndorser)) {
        for (const element of roleTypes.intermediateEndorser) {
          const customerId = element.substring(0, element.indexOf("-"));
          chequeRole = { customerId, roleType };
          chequeRoles.push(chequeRole);
        }
      } else {
        const customerId = roleTypes.intermediateEndorser.substring(
          0,
          roleTypes.intermediateEndorser.indexOf("-")
        );
        chequeRole = { customerId, roleType };
        chequeRoles.push(chequeRole);
      }
    }

    if (roleTypes.lastEndorser !== undefined) {
      let chequeRole = {};
      let roleType = "LAST_ENDORSER";
      if (Array.isArray(roleTypes.lastEndorser)) {
        for (const element of roleTypes.lastEndorser) {
          const customerId = element.substring(0, element.indexOf("-"));
          chequeRole = { customerId, roleType };
          chequeRoles.push(chequeRole);
        }
      } else {
        const customerId = roleTypes.lastEndorser.substring(
          0,
          roleTypes.lastEndorser.indexOf("-")
        );
        chequeRole = { customerId, roleType };
        chequeRoles.push(chequeRole);
      }
    }

    return chequeRoles;
  };

  const onClear = () => {
    formRefCustomer.current.resetFields();
    formRefChequeInfo.current.resetFields();
    formRefDates.current.resetFields();
    dispatch(setChequeInfos([]));
  };

  const onSubmit = async () => {
    dispatch(setTableLoading(true));

    let chequeNos = formRefChequeInfo.current.getFieldsValue().chequeNo;
    let chequeRegistryIds = formRefDates.current.getFieldsValue().registerNo;
    const minNumber = formRefChequeInfo.current.getFieldsValue().minNumber;
    const maxNumber = formRefChequeInfo.current.getFieldsValue().maxNumber;

    const amountRange = { minNumber, maxNumber };

    if (chequeNos !== undefined && chequeNos.length > 0) {
      chequeNos = chequeNos.map((number) => {
        return Number(number);
      });
    }

    if (chequeRegistryIds !== undefined && chequeRegistryIds.length > 0) {
      chequeRegistryIds = chequeRegistryIds.map((number) => {
        return Number(number);
      });
    }

    const chequeRoles = getRoleTypesAndCustomerNos();

    // DATE
    const formValues = formRefDates.current.getFieldsValue();

    let startDate = null;

    if (
      formValues.issueDate !== null &&
      formValues.issueDate[0] !== undefined
    ) {
      startDate = formValues.issueDate[0]._d;
      startDate = moment(startDate).utcOffset(180).startOf("day");
    }

    let endDate = null;
    if (
      formValues.issueDate !== null &&
      formValues.issueDate[1] !== undefined
    ) {
      console.log(
        "formValues.issueDate.endDate: ",
        formValues.issueDate.endDate
      );

      endDate = formValues.issueDate[1]._d;
      endDate = moment(endDate).utcOffset(180).endOf("day");
    }

    //if()
    let result = endDate.diff(startDate, "days");

    console.log("No of Days:", result);

    if (result > 90) {
      Message.error({ content: "tarih araligi 3 aydan fazla" });
      return;
    }

    const issueDateRange = { startDate, endDate };

    console.log("issueDateRange: ", issueDateRange);

    let createStartDate = null;
    if (
      formValues.createDate !== null &&
      formValues.createDate[0] !== undefined
    ) {
      console.log(
        "formValues.createDate.startDate: ",
        formValues.createDate.startDate
      );

      createStartDate = formValues.createDate[0]._d;
      createStartDate = moment(createStartDate).utcOffset(180).startOf("day");
    }

    let createEndDate = null;
    if (
      formValues.createDate !== null &&
      formValues.createDate[1] !== undefined
    ) {
      console.log(
        "formValues.createDate.endDate: ",
        formValues.createDate.endDate
      );

      createEndDate = formValues.createDate[1]._d;
      createEndDate = moment(createEndDate).utcOffset(180).endOf("day");
    }

    startDate = createStartDate;
    endDate = createEndDate;

    const creationDateRange = { startDate, endDate };

    let customerId = null;
    const customerField = formRefCustomer.current.getFieldsValue().customer;
    if (customerField !== undefined) {
      customerId = customerField.substring(0, customerField.indexOf("-") - 1);
      customerId = Number(customerId);
    }

    const chequeInfoformValues = formRefChequeInfo.current.getFieldsValue();
    let channelCodes = [];

    if (chequeInfoformValues.channelCodes) {
      channelCodes = chequeInfoformValues.channelCodes.map((code) => {
        return code;
      });
    }

    const body = {
      customerId,
      chequeRoles,
      chequeNos,
      chequeRegistryIds,
      amountRange,
      issueDateRange,
      creationDateRange,
      channelCodes,
    };

    getChequeInfos(body);
  };

  const getChequeInfos = (body) => {
    const url = "/fctr-cheque-bff/search-by-filter";
    api
      .post(url, body)
      .then((response) => {
        dispatch(setChequeInfos(response.data));
        dispatch(setTableLoading(false));
      })
      .catch((err) => {})
      .finally(() => {
        // to do
      });
  };

  const handleSetCustomers = () => {
    // setCustomerNameSurname(customer["customerIdentity"]);

    const newData = {
      a:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
      b:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
    };
    formRefCustomer.current.setFieldsValue({
      customer: newData.a,
    });

    dispatch(setCustomerList([]));
    dispatch(setCustomerList(customerList.concat(newData)));
  };

  const handleSetDrawers = () => {
    //  setCustomerNameSurname(customer["customerIdentity"]);

    const newData = {
      a:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
      b:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
    };
    dispatch(setDrawerList(drawerList.concat(newData)));

    formRefCustomer.current.setFieldsValue({
      drawer: newData.a,
    });
  };

  const handleSetLastEndorsers = () => {
    //  setCustomerNameSurname(customer["customerIdentity"]);

    const newData = {
      a:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
      b:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
    };
    dispatch(setLastEndorserList(lastEndorserList.concat(newData)));
    formRefCustomer.current.setFieldsValue({
      lastEndorser: newData.a,
    });
  };

  const handleSetIntermediateEndorsers = () => {
    //  setCustomerNameSurname(customer["customerIdentity"]);

    const newData = {
      a:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
      b:
        customer.customerNo +
        " - " +
        customer["customerIdentity"].name +
        " - " +
        customer["customerIdentity"].surname,
    };

    dispatch(
      setIntermediateEndorserList(intermediateEndorserList.concat(newData))
    );
    formRefCustomer.current.setFieldsValue({
      intermediateEndorser: newData.a,
    });
  };

  const handleChangeCustomer = (value) => {
    // to do
  };

  const handleChangeDrawer = (value) => {
    // to do
  };

  const handleChangeIntermediateEndorser = (value) => {
    // to do
  };

  const handleChangeLastEndorser = (value) => {
    // to do
  };

  const channelData = [
    {
      key: "APP",
      value: "APP",
    },
    {
      key: "MAIL",
      value: "MAIL",
    },
    {
      key: "NONE",
      value: "NONE",
    },
  ];

  const currencyData = [
    {
      key: "TL",
      value: "TL",
    },
    {
      key: "USD",
      value: "USD",
    },
    {
      key: "EUR",
      value: "EUR",
    },
  ];

  const intelligenceInfoData = [
    {
      key: "Tümü",
      value: "TÜMÜ",
    },
    {
      key: "olumlu",
      value: "OLUMLU",
    },
    {
      key: "olumsuz",
      value: "OLUMSUZ",
    },
  ];

  return (
    <>
      <StyledContent>
        <div>MÜŞTERİ</div>
      </StyledContent>

      <FactorCustomerSearch
        withPermission={withPermission}
        usePermission={usePermission}
        useShellCommunicator={useShellCommunicator}
        onChange={(customer) => {
          setCustomer(customer);
        }}
      />
      <StyledContent>
        <div
          style={{
            display: "flex",
            justifyContent: "flex-end",
            alignItems: "center",
            marginTop: "20px",
          }}
        >
          <Button type="secondary" onClick={handleSetCustomers}>
            {i18n.addAsCustomer}
          </Button>
          <Button type="secondary" onClick={handleSetDrawers}>
            {i18n.addAsDrawer}
          </Button>
          <Button type="secondary" onClick={handleSetIntermediateEndorsers}>
            {i18n.addAsEndorser}
          </Button>
          <Button type="secondary" onClick={handleSetLastEndorsers}>
            {i18n.addAsLastEndorser}
          </Button>
        </div>
      </StyledContent>
      <StyledContent>
        <div style={{ marginBottom: "15px" }}>
          <div>
            <StyledContent>
              <Form ref={formRefCustomer}>
                <Form.Item label={i18n.customername} colSpan={quarterSpan}>
                  {/*<Tag closable name="customer" data={customer}></Tag>*/}
                  <Select
                    name="customer"
                    onChange={handleChangeCustomer}
                    data={customerList}
                    valueKey={"a"}
                    labelKey={"b"}
                    defaultValue={[]}
                  />
                </Form.Item>

                <Form.Item label={i18n.drawer.s} colSpan={quarterSpan}>
                  <Select
                    name="drawer"
                    showSelectAll={true}
                    onChange={handleChangeDrawer}
                    data={drawerList}
                    mode="multiple"
                    valueKey={"a"}
                    labelKey={"b"}
                    defaultValue={[]}
                  />
                </Form.Item>
                <Form.Item
                  label={i18n.intermediateendorser}
                  colSpan={quarterSpan}
                >
                  <Select
                    name="intermediateEndorser"
                    onChange={handleChangeIntermediateEndorser}
                    data={intermediateEndorserList}
                    mode="multiple"
                    valueKey={"a"}
                    labelKey={"b"}
                    defaultValue={[]}
                  />
                </Form.Item>
                <Form.Item label={i18n.lastendorser} colSpan={quarterSpan}>
                  <Select
                    name="lastEndorser"
                    onChange={handleChangeLastEndorser}
                    data={lastEndorserList}
                    mode="multiple"
                    valueKey={"a"}
                    labelKey={"b"}
                    defaultValue={[]}
                  />
                </Form.Item>
              </Form>
            </StyledContent>
          </div>
          <div>
            <StyledContent>
              <Form ref={formRefChequeInfo}>
                <Form.Item label={i18n.cheque.no} colSpan={quarterSpan}>
                  <Select
                    name="chequeNo"
                    mode="tags"
                    maxTagTextLength={50}
                    maxTagCount={3}
                    placeholder=""
                  ></Select>
                </Form.Item>

                <Form.Item
                  label={i18n.cheque.minChequeAmount}
                  colSpan={oneOfEightSpan}
                >
                  <TextInput name="minNumber" />
                </Form.Item>
                <Form.Item
                  label={i18n.cheque.maxChequeAmount}
                  colSpan={oneOfEightSpan}
                >
                  <TextInput name="maxNumber" />
                </Form.Item>
                <Form.Item label={i18n.currencyUnit} colSpan={quarterSpan}>
                  <Select
                    name="currencyUnit"
                    data={currencies}
                    valueKey={"key"}
                    labelKey={"value"}
                  />
                </Form.Item>

                <Form.Item label={i18n.cheque.channel} colSpan={quarterSpan}>
                  <Select
                    name="channelCodes"
                    mode="multiple"
                    data={channelData}
                    valueKey={"key"}
                    labelKey={"value"}
                  />
                </Form.Item>
              </Form>
            </StyledContent>
          </div>
          <div>
            <StyledContent>
              <Form ref={formRefDates}>
                <Form.Item
                  label={i18n.cheque.chequeIntelligenceResultInfo}
                  colSpan={quarterSpan}
                >
                  <Select
                    name="chequeIntelligenceResultInfo"
                    data={intelligenceInfoData}
                    valueKey={"key"}
                    labelKey={"value"}
                  />
                </Form.Item>
                <Form.Item label={i18n.cheque.registerNo} colSpan={quarterSpan}>
                  <Select
                    name="registerNo"
                    mode="tags"
                    maxTagTextLength={50}
                    maxTagCount={3}
                  ></Select>
                </Form.Item>
                <Form.Item label={i18n.expiryDate} colSpan={quarterSpan}>
                  <RangePicker
                    name="issueDate"
                    placeholder={{
                      startDate: i18n.cheque.startDate,
                      endDate: i18n.cheque.endDate,
                    }}
                  />
                </Form.Item>
                <Form.Item label={i18n.cheque.createDate} colSpan={quarterSpan}>
                  <RangePicker
                    name="createDate"
                    placeholder={{
                      startDate: i18n.cheque.startDate,
                      endDate: i18n.cheque.endDate,
                    }}
                  />
                </Form.Item>
              </Form>
            </StyledContent>
          </div>

          <div
            style={{
              display: "flex",
              justifyContent: "right",
              alignItems: "center",
            }}
          >
            <Button onClick={onClear} type="secondary">
              {i18n.clear}
            </Button>
            <Button onClick={onSubmit} type="primary">
              {i18n.list}
            </Button>
          </div>
        </div>
      </StyledContent>
    </>
  );
};

export default ChequeQueryFilter;
