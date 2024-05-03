import { Button, Icon, Modal, Table, TableHeader } from "ykb-ui";
import { exportToExcel } from "ykb-utils";
import { useDispatch, useSelector } from "react-redux";
import { useIntl } from "factor-shell";
import { useState } from "react";
import ChequeUpdateModal from "./ChequeUpdateModal";
import {
  setChequeId,
  setChequeInfos,
  setDeletedCheques,
  setEditCheque,
} from "../../store/chequeQuerySlice";
import ChequeViewModal from "./ChequeViewModal";
import { stringSorter } from "../../utils";

const moment = require("moment");

const ChequeQueryTable = (props) => {
  const { i18n } = useIntl();

  const [updateModalVisible, setUpdateModalVisible] = useState(false);
  const [viewModalVisible, setViewModalVisible] = useState(false);

  const { chequeInfos, tableLoading, deletedCheques, banks, branches } =
    useSelector(({ chequeQuery }) => chequeQuery);

  const dispatch = useDispatch();

  const [fileName, setFileName] = useState("");

  // şimdilik response'u olmayan kolonların dataIndex'i boş olarak bırakıldı
  const columns = [
    {
      title: i18n.cheque.registerNo,
      dataIndex: "registryNo",
      width: 300,
    },
    {
      title: i18n.cheque.customerNo,
      dataIndex: "customerDetail?",
      render: (text, record) => {
        const { customerDetail } = record;
        if (customerDetail === undefined || customerDetail === null) return "";
        return `${customerDetail?.customerNo}`;
      },
    },
    {
      title: i18n.cheque.customerNameSurnameTitle,
      dataIndex: "customerDetail?",
      width: 300,
      render: (text, record) => {
        const { customerDetail } = record;
        if (customerDetail === undefined || customerDetail === null) return "";
        const title = customerDetail.commercialTitle
          ? customerDetail.commercialTitle
          : "";
        let result = "";
        if (title !== "")
          result = `${customerDetail?.name} - ${customerDetail?.surname} - ${title}`;
        else result = `${customerDetail?.name} - ${customerDetail?.surname}`;

        return result;
      },
    },
    {
      title: i18n.cheque.channel,
      dataIndex: "channelCode",
    },

    {
      title: i18n.cheque.no,
      dataIndex: "chequeNo",
    },
    {
      title: i18n.currencyUnit,
      dataIndex: "currency",
      width: 300,
    },
    {
      title: i18n.cheque.amount,
      dataIndex: "amount",
      render: (amount) => {
        return parseFloat(amount) + ".00";
      },
    },
    {
      title: i18n.cheque.amountTL,
      dataIndex: "amountTL",
    },

    {
      title: i18n.cheque.expiration,
      dataIndex: "issueDate",
      render: (issueDate) =>
        issueDate
          ? moment(issueDate).utcOffset(180).format("DD/MM/YYYY HH:mm:ss")
          : "",
    },

    {
      title: i18n.cheque.bankCode,
      dataIndex: "bankCode",
      sorter: stringSorter,
      //defaultSortOrder: "descend",
      render: (record) => {
        let bankName = "";
        for (let i = 0; i < banks.length; i++) {
          if (banks[i]["bankCode"] === record) {
            bankName = banks[i]["bankName"];
            break;
          }
        }
        return record + " - " + bankName;
      },
    },
    {
      title: i18n.cheque.branchCode,
      dataIndex: "branchCode",
      // TO DO: Şube kod servisi (bankCode) paramtetresi ile sonuç dönüyor.
      //         tüm şubeleri alacak api lazım

      /*render: (record) => {

        console.log("record: ", record);
        console.log("branches: ", branches);

        let bankName = "";
        for (let i = 0; i < branches.length; i++) {
          if (branches[i]["bankCode"] === record) {
            bankName = banks[i]["bankName"];
            break;
          }
        }
        return record + " - " + bankName;
      },*/
    },
    {
      title: i18n.cheque.drawerNo,
      dataIndex: "drawerDetail?",
      render: (text, record) => {
        const { drawerDetail } = record;
        if (drawerDetail === undefined || drawerDetail === null) return "";
        return `${drawerDetail?.customerNo}`;
      },
    },
    {
      title: i18n.cheque.drawerNameSurname,
      dataIndex: "drawerDetail?",
      render: (text, record) => {
        const { drawerDetail } = record;
        if (drawerDetail === undefined || drawerDetail === null) return "";
        return `${drawerDetail?.name} - ${drawerDetail?.surname}`;
      },
    },
    {
      title: i18n.cheque.drawerTckn,
      dataIndex: "drawerDetail.tcknVkn",
    },
    {
      title: i18n.description,
      dataIndex: "",
    },
    {
      title: i18n.cheque.endorserNo,
      dataIndex: "endorserDetail?",
      render: (text, record) => {
        const { endorserDetail } = record;
        if (endorserDetail === undefined || endorserDetail === null) return "";
        return `${endorserDetail?.customerNo}`;
      },
    },
    {
      title: i18n.cheque.endorserNameSurname,
      dataIndex: "endorserDetail?",
      render: (text, record) => {
        const { endorserDetail } = record;
        if (endorserDetail === undefined || endorserDetail === null) return "";
        return `${endorserDetail?.name} - ${endorserDetail?.surname}`;
      },
    },
    {
      title: i18n.cheque.endorserTckn,
      dataIndex: "endorserDetail.tcknVkn",
    },
    {
      title: i18n.cheque.createDate,
      dataIndex: "createDate",
      render: (createDate) =>
        createDate
          ? moment(createDate).utcOffset(180).format("DD/MM/YYYY HH:mm:ss")
          : "",
    },
    {
      title: i18n.cheque.lastOperationDate,
      dataIndex: "createDate",
      render: (createDate) =>
        createDate
          ? moment(createDate).utcOffset(180).format("DD/MM/YYYY HH:mm:ss")
          : "",
    },
    {
      title: i18n.exchangeRate,
      dataIndex: "exchangeRate",
    },
    {
      title: i18n.cheque.collectCurrency,
      dataIndex: "exchangeRate",
    },

    {
      title: i18n.cheque.additionNo,
      dataIndex: "",
    },
    {
      title: i18n.cheque.totalAmount,
      dataIndex: "",
    },
    {
      title: i18n.cheque.remainingAmount,
      dataIndex: "",
    },

    {
      title: i18n.cheque.valueDate,
      dataIndex: "",
    },
    {
      title: i18n.cheque.lastUpdatedBy,
      dataIndex: "updatedBy",
    },

    {
      title: i18n.cheque.KKBRefNo,
      dataIndex: "",
    },
    {
      title: i18n.cheque.accountBranchCode,
      dataIndex: "",
    },
    {
      title: i18n.cheque.sendBankNo,
      dataIndex: "",
    },
    {
      title: i18n.cheque.sendBankAddNo,
      dataIndex: "",
    },
    {
      title: i18n.givingPlace,
      dataIndex: "",
    },
    {
      title: i18n.cheque.priceRef,
      dataIndex: "",
    },
    {
      title: i18n.cheque.customerRepresentative,
      dataIndex: "",
    },

    {
      title: i18n.cheque.loginUserNameSurname,
      dataIndex: "createdBy",
    },
    {
      title: i18n.cheque.autoIntelligenceResult,
      dataIndex: "",
    },
    {
      title: i18n.cheque.intelligenceResult,
      dataIndex: "",
    },
    {
      title: i18n.cheque.intelligenceDescription,
      dataIndex: "",
    },
    {
      title: i18n.cheque.intelligenceType,
      dataIndex: "",
    },
    {
      title: "view",
      dataIndex: "",
      width: "55px",
      type: {
        action: "detail",
        handler: (record) => {
          setFileName(record.channelCode + ".png");
          console.log("DOCUMENT ID:  ", record.documentId);
          dispatch(setChequeId(record.documentId));
          setViewModalVisible(true);
        },
      },
    },
    {
      dataIndex: "",
      width: "55px",
      type: {
        action: "edit",
        handler: (record) => {
          console.log("editt");
          dispatch(setEditCheque(record));
          setUpdateModalVisible(true);
          console.log("ÇEK RECORD: ", record);
        },
      },
    },
    {
      dataIndex: "",
      width: "255px",
      type: {
        action: "delete",
        handler: (record) => {
          const remainingCheques = chequeInfos.filter(
            (item) => item.id !== record.id
          );
          dispatch(setChequeInfos(remainingCheques));
          dispatch(setDeletedCheques(deletedCheques.concat(record.id)));
        },
      },
    },
  ];

  /* onExcelExport={() => {
    exportToExcel({
      columns: columns,
      data: chequeInfos,
      filename: i18n.cheque.excelHeading,
    });
  }} */

  function handleExportToExcel() {
    exportToExcel({
      columns: columns,
      data: chequeInfos,
      filename: i18n.cheque.information,
    });
  }

  function RenderTableHeaderExtra({ handleExportToExcel }) {
    const { i18n } = useIntl();

    return (
      <Button type="primary" size="small" onClick={handleExportToExcel}>
        {i18n.exportExcel}
      </Button>
    );
  }
  function RenderTableHeader({ handleExportToExcel }) {
    const { i18n } = useIntl();

    return (
      <TableHeader
        title={i18n.cheque.excelResult}
        extra={
          <RenderTableHeaderExtra handleExportToExcel={handleExportToExcel} />
        }
      />
    );
  }

  return (
    <>
      <ChequeUpdateModal
        updateModalVisible={updateModalVisible}
        handleClose={() => {
          setUpdateModalVisible(false);
        }}
      ></ChequeUpdateModal>

      <ChequeViewModal
        DocumentViewer={props.DocumentViewer}
        fileName={fileName}
        viewModalVisible={viewModalVisible}
        handleClose={() => {
          setViewModalVisible(false);
        }}
      ></ChequeViewModal>

      <Table
        title={<RenderTableHeader handleExportToExcel={handleExportToExcel} />}
        style={{
          whiteSpace: "nowrap",
          display: "inline",
        }}
        rowKey="id"
        columns={columns}
        data={chequeInfos}
        scroll={{ x: true }}
        loading={tableLoading}
      />
    </>
  );
};

export default ChequeQueryTable;
