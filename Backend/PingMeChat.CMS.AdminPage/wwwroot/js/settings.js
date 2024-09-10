const evn = {
    apiUrl: 'http://localhost:5009/', // địa chỉ API 
    appUrl: 'http://localhost:5262/', // địa chỉ API

  



    api: {
        curriculumVitae: {
            getAll: 'api/feature/curriculum-vitae/getAll',
        },
        group: {
            searchGroupRoute: 'api/feature/group/search',
            getAllGroupRoute: 'api/feature/group/all',
            createGroupRoute: 'api/feature/group/create',
        }
    },
    app: {
        loginUrl: '', // địa chỉ trang đăng nhập
        logoutUrl: 'Account/Logout', // địa chỉ trang đăng xuất
        admin: {
            loaiDon: 'admin/loaiDon/GetAll',
        },
        role: {
            getAll: 'role/getAll',
            getById: 'role/getById/{id}',
            update_status: 'role/update-status/',
            update: 'role/update/',
        },
        loaiDon: {
            getAsync: '/LoaiDon/GetAsync/{id}',
            pageStatus: '/LoaiDon/PageStatus',
        },
        donDaDangKy: {
            pageIndex: '/danh-sach-don-da-dang-ky',
            initialData: '/DonDaDangKy/InitialData',
            search: '/DonDaDangKy/DanhSachDonDaDangKy',
        },
        dxcbd: {
            createAsync: 'DXCBD/CreateAsync',
        },
       
        donVi: {
            getAllKhoa: '/don-vi/danh-sach-khoa',
            getAllNganh: '/don-vi/danh-sach-nganh',
            getAllNganhByKhoa: '/don-vi/danh-sach-nganh-thuoc-khoa/',
        },

        // ---
        form: {
            formType: {
                getAll: '/form/form-type/get-all',
                loadDataToViewHome: "/form/form-type/load-data-to-view-home",
                loadDataToViewForm: "/form/form-type/load-data-to-view-form",
            },
            dxcbd: {
                create: '/form/dxcbd/create'
            },
            dxlltsv: {
                create: '/form/dxlltsv/create',
            },
            dxnlsvdhtt: {
                create: '/form/dxnlsvdhtt/create'
            },
            ddkhp: {
                create: '/form/ddkhp/create',
                bindDataToTableWhenAddTerm : '/form/ddkhp/bind-data-to-table-when-add-term'
            }
            
        },
        student: {
            getById: '/student/get-by-id/{id}'
        },
        courseSchedule: {
            create: "/CourseSchedule/Create",
        },
        service: {
            captcha: {
                getCaptcha: '/captcha/get-captcha'
            }
        },
        extension: {
            generateCode: '/Extension/GetGenerateCode'
        } 
    }
};