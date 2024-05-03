import { Button, Col, Message, Row } from "ykb-ui";

import ChequeQueryFilter from "./ChequeQueryFilter";
import ChequeQueryTable from "./ChequeQueryTable";
import { useIntl } from "factor-shell";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  getBankListRequest,
  getBranchListRequest,
  getCurrenciesRequest,
  getOfficialRatesRequest,
} from "../../store/chequeQuerySlice";
import {
  setDeletedCheques,
  setUpdatedCheques,
} from "../../store/chequeQuerySlice";
import api from "../../api";

const ChequeQueryForm = (props) => {
  const { i18n } = useIntl();

  const dispatch = useDispatch();

  const { updatedCheques, deletedCheques } = useSelector(
    ({ chequeQuery }) => chequeQuery
  );

  useEffect(() => {
    dispatch(getCurrenciesRequest());
    dispatch(getOfficialRatesRequest());
    dispatch(getBankListRequest());
    // TO DO: şu an tek bir bankanın şubeleri çekiliyor
    dispatch(getBranchListRequest("103"));
  }, [dispatch]);

  const updateChequeInfos = (body) => {
    const url = "/fctr-cheque-bff/change-in-batch";
    api
      .post(url, body)
      .then((response) => {
        console.log("RESPONSEE: ", response);
        Message.info({
          content: i18n.cheque.chequeUpdateOk,
        });
        // çek infos u güncelle (ekranda refresh icin)
        //dispatch(setChequeInfos(response.data));
        //dispatch(setTableLoading(false));
      })
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        // to do
      });
  };

  const handleSave = () => {
    let updateRequest = {
      chequeIdsToDelete: deletedCheques,
      chequeDTOSToUpdate: updatedCheques,
    };

    updateChequeInfos(updateRequest);

    // kaydete basınca işlem tamam ise response olumlu ise -->  deletedCheques ve updatedCheques i sıfırla. şimdilik daima sıfırlıyorum
    dispatch(setUpdatedCheques([]));
    dispatch(setDeletedCheques([]));
  };

  const handleCancel = () => {
    console.log("CANCEL");
  };

  return (
    <>
      <Row>
        <Col>
          <ChequeQueryFilter {...props} />
        </Col>
      </Row>
      <Row>
        <Col>
          <ChequeQueryTable DocumentViewer={props.DocumentViewer} />
        </Col>
      </Row>
      <Row
        style={{
          display: "flex",
          justifyContent: "flex-end",
          alignItems: "center",
          marginTop: "10px",
        }}
      >
        <Button type="primary" onClick={handleCancel}>
          {i18n.cancel}
        </Button>
        <Button type="primary" onClick={handleSave}>
          {i18n.save}
        </Button>
      </Row>
    </>
  );
};

export default ChequeQueryForm;
