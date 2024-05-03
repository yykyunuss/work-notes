export const getDateObject = (dt) => {
  if (!dt) return null;

  if (typeof dt.getMonth === 'function') {
    return dt;
  }

  let date = null;

  if (typeof dt === 'string' || typeof dt === 'number') {
    if (typeof dt === 'string') {
      const splittedDate = dt.split('.');
      if (splittedDate.length === 3 && splittedDate[2].length === 4) {
        date = `${splittedDate[1]}.${splittedDate[0]}.${splittedDate[2]}`;
      }
      date = new Date(date);
    } else {
      date = new Date(dt);
    }
  }

  if (Array.isArray(dt)) {
    const [fullYear, month2, day2, hours2, mins2, seconds2] = dt;
    const day = (day2 < 10 ? '0' : '') + day2;
    const month = (month2 < 10 ? '0' : '') + month2;
    const hours = (hours2 < 10 ? '0' : '') + hours2;
    const mins = (mins2 < 10 ? '0' : '') + mins2;
    const secs = (seconds2 < 10 ? '0' : '') + seconds2;

    date = new Date(`${fullYear}-${month}-${day}T${hours}:${mins}:${secs}`);
  }

  try {
    if (date.toString() === 'Invalid Date') {
      return null;
    }
  } catch (err) {
    return null;
  }

  return date;
};

export const getFormattedDate = (dt, excludeTime) => {
  const date = getDateObject(dt);

  if (!date) return '';

  try {
    if (date.toString() === 'Invalid Date') {
      return '';
    }
  } catch (err) {
    return '';
  }

  let formattedDate = '';

  try {
    const day = (date.getDate() < 10 ? '0' : '') + date.getDate();
    let month = date.getMonth() + 1;
    month = (month < 10 ? '0' : '') + month;
    const fullYear = date.getFullYear();
    const hours = (date.getHours() < 10 ? '0' : '') + date.getHours();
    const mins = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
    const secs = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
  
    formattedDate = `${day}.${month}.${fullYear} ${excludeTime ? '' : (hours + ':' + mins + ':' + secs)}`;
  } catch (ex) {
    console.log(ex);
  }

  return formattedDate;
};

export const formatNumber = (number, precision) => {
  if (number === null || number === undefined) return '';
  let numberToFormat = number;
  if (typeof numberToFormat === 'string' && (!numberToFormat || isNaN(numberToFormat))) return '';

  const formatProps = {};

  if (precision) {
    formatProps['minimumSignificantDigits'] = precision;
    formatProps['maximumSignificantDigits'] = precision;
  }
  
  try {
    numberToFormat = parseFloat(numberToFormat);
  } catch (err) {
    return '';
  }

  return new Intl.NumberFormat('TR-tr', formatProps).format(numberToFormat);
};

export const halfSpan = { xs: 24, md: 12 };
export const threeQuarterSpan = { xs: 24, md: 16 };
export const oneOfThreeSpan = { xs: 24, sm: 12, md: 8 };
export const quarterSpan = { xs: 24, sm: 12, md: 6 };
export const fullSpan = { span: 24 };

export const replaceTransalation = (translation, ...words) => {
  let translatedText = translation;
  for (let word of words) {
    translatedText = translatedText.replace('%s', word);
  }

  return translatedText;
};

export const getCustomerName = (customer) => {
  if (!customer) return '';

  if (customer.type === '210') {
    return customer.customerCommercialInfo?.corporateName;
  } else {
    return customer.customerIdentity?.name +
          " " +
          customer.customerIdentity?.surname
  }
};

export const getCustomerTcknVkn = (customer) => {
  if (!customer) return '';

  if (customer.type === '210') {
    return customer.customerCommercialInfo?.tin;
  } else {
    return customer.customerIdentity?.identityNo;
  }
};

export const ChequeRole = {
  DRAWER: 'DRAWER',
  ENDORSER: 'ENDORSER',
  INTERMEDIATE_ENDORSER: 'INTERMEDIATE_ENDORSER',
};

export const getDrawer = (cheque) => {
  if (!cheque?.chequeRoles?.length) return null;

  return cheque.chequeRoles.find((cr) => cr.roleTypeCode === ChequeRole.DRAWER)?.customer;
};

export const getEndorser = (cheque) => {
  if (!cheque?.chequeRoles?.length) return null;

  return cheque.chequeRoles.find((cr) => cr.roleTypeCode === ChequeRole.ENDORSER)?.customer;
};

export const getIntermediateEndorsers = (cheque) => {
  if (!cheque?.chequeRoles?.length) return [];

  return cheque.chequeRoles.filter((cr) => cr.roleTypeCode === ChequeRole.INTERMEDIATE_ENDORSER).map((cr) => cr.customer);
};

export const getRemaininIssueDateDays = (cheque) => {
  if (!cheque?.issueDate) return 0;

  const issueDate = new Date(cheque.issueDate);
  const now = new Date();

  const timeInMillisecs = issueDate.getTime() - now.getTime();

  let result = timeInMillisecs / 1000 / 60 / 24;

  return Math.floor(result);
};

export const convertBase64ToBlob = async (base64String, mimeType) => {
  return new Promise((resolve, reject) => {
    fetch(`data:${mimeType};base64,${base64String}`)
    .then((res) => res.blob())
    .then((blob) => resolve(blob))
    .catch((err) => reject(err));
  });
};

export const ExcelField = {
  chequeNo: 'CHEQUE_NO',
  amount: 'AMOUNT',
  issueDate: 'ISSUE_DATE',
  bankCode: 'BANK_CODE',
  branchCode: 'BRANCH_CODE',
  chequeAccountNo: 'CHEQUE_ACCOUNT_NO',
  drawerNo: 'DRAWER_NO',
  drawerTcknVkn: 'DRAWER_TCKN_VKN',
  endorserNo: 'ENDORSER_NO',
  endorserTcknVkn: 'ENDORSER_TCKN_VKN',
  currency: 'CURRENCY',
};

export const stringSorter = (a, b, sortOrder, sortColumn) => {
  const aValue = a[sortColumn.dataIndex];
  const bValue = b[sortColumn.dataIndex];

  if (aValue < bValue) {
    return -1;
  } else if (aValue > bValue) {
    return 1;
  }

  return 0;
};

export const numberSorter = (a, b, sortOrder, sortColumn) => {
  const aValue = a[sortColumn.dataIndex];
  const bValue = b[sortColumn.dataIndex];

  return aValue - bValue;
};

export const dateSorter = (a, b, sortOrder, sortColumn) => {
  const aValue = a.updateDate ? a.updateDate : a.createDate;
  const bValue = b.updateDate ? b.updateDate : b.createDate;

  return aValue - bValue;
};

export const getDrawerVkn = (customer) => {
  const tcknVkn = getCustomerTcknVkn(customer)

  if (!tcknVkn) return '';

  return tcknVkn.toString().length === 10 ? tcknVkn : '';
};

export const getDrawerTckn = (customer) => {
  const tcknVkn = getCustomerTcknVkn(customer)
  if (!tcknVkn) return '';

  return tcknVkn.toString().length === 11 ? tcknVkn : '';
};

export function orderBy(arr, keyFunction, sortOrder = 'ASC') {
  return arr.slice().sort((a, b) => {
    const keyA = keyFunction(a);
    const keyB = keyFunction(b);

    const comparison = keyA.localeCompare(keyB);

    return sortOrder === 'ASC' ? comparison : -comparison;
  });
}

export function fillDataWithKey(data) {
  return (data && data.length) ? data.map((val, i) => ({ key: i, ...val })) : data;
}

export function prepareCreatorInfo(user) {
  let prettyUserInfo = '';
  if (user) {
    prettyUserInfo = `${user.userCode} - ${user.userName}`;
  }

  return ({
    createdBy: prettyUserInfo,
    updatedBy: prettyUserInfo,
    createDate: new Date(),
    updateDate: new Date(),
  });
}

export function dateCompare(arr, keyFunction, sortOrder = 'ASC') {
  return [...arr].sort((a, b) => {
    const keyA = keyFunction(a);
    const keyB = keyFunction(b);

    const comparison = (new Date(keyA)?.getTime() ?? 0) - (new Date(keyB)?.getTime() ?? 0);

    return sortOrder === 'ASC' ? comparison : -comparison;
  });
}


export const customerRoleTypes = {
  DRAWER: "DRAWER",
  ENDORSER: "ENDORSER",
  LAST_ENDORSER: "LAST_ENDORSER",
};

export const getBlob = (base64) => {
  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);
  const blob = new Blob([byteArray], { type: "image/png" });

  console.log("blob in method:  ", blob);

  return blob;
};

export const convertBase64ToBlob = async (base64String, mimeType) => {
  return new Promise((resolve, reject) => {
    fetch(`data:${mimeType};base64,${base64String}`)
      .then((res) => res.blob())
      .then((blob) => resolve(blob))
      .catch((err) => reject(err));
  });
};

export const oneOfEightSpan = { xs: 24, sm: 12, md: 3 };
export const halfSpan = { xs: 24, md: 12 };
export const threeQuarterSpan = { xs: 24, md: 16 };
export const oneOfThreeSpan = { xs: 24, sm: 12, md: 8 };
export const quarterSpan = { xs: 24, sm: 12, md: 6 };
export const fullSpan = { span: 24 };




