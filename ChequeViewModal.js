import { useIntl } from "factor-shell";
import { useEffect, useState } from "react";
import { Modal, Spin } from "ykb-ui";
import api from "../ChequeQuery/api";
import { styled } from "ykb-ui/lib/ykb-styled-components";
import { useSelector } from "react-redux";
import { getBlob } from "./utils";

const StyledViewDocument = styled.div`
  .webviewer {
    height: 100vh;
  }
`;

const ChequeViewModal = (props) => {
  const { DocumentViewer, viewModalVisible, fileName, handleClose } = props;

  const { chequeId } = useSelector(({ chequeQuery }) => chequeQuery);

  const [blobFile, setBlobFile] = useState();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (chequeId !== 0) {
      handleViewCheque();
    }
  }, [chequeId]);

  const handleViewCheque = () => {
    setLoading(true);

    downloadBlobData()
      .then((blob) => {
        setBlobFile({ blob: blob, fileName: fileName });
      })
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  const downloadBlobData = async () => {
    const url = `https://fctrgateway-tst.factoring.yapikredi.com.tr/fctr-gateway-bff/cosmos-dms-bff/document/content/${chequeId}`;
    return api
      .get(url)
      .then((response) => getBlob(response.data.data.content))
      .catch((err) => {
        console.log(err);
      })
      .finally(() => {
        // to do
      });
  };

  return (
    <Modal visible={viewModalVisible} onClose={handleClose}>
      <StyledViewDocument>
        <Spin spinning={loading}>
          {blobFile && blobFile.blob && (
            <>
              <DocumentViewer.Component
                documentSource={blobFile.blob}
                fileName={blobFile.fileName}
              />
            </>
          )}
        </Spin>
      </StyledViewDocument>
    </Modal>
  );
};

export default ChequeViewModal;
