Public Class JobTaskParsed
    Private _couponCode As String
    Public Property CouponCode() As String
        Get
            Return _couponCode
        End Get
        Set(ByVal value As String)
            _couponCode = value
        End Set
    End Property

    Private _taxRate As Decimal
    Public Property TaxRate() As Decimal
        Get
            Return _taxRate
        End Get
        Set(ByVal value As Decimal)
            _taxRate = value
        End Set
    End Property

    Private _discountReason As String
    Public Property DiscountReason() As String
        Get
            Return _discountReason
        End Get
        Set(ByVal value As String)
            _discountReason = value
        End Set
    End Property

    Private _taxName As String
    Public Property TaxName() As String
        Get
            Return _taxName
        End Get
        Set(ByVal value As String)
            _taxName = value
        End Set
    End Property

    Private _Successful As Boolean
    Public Property Successful() As Boolean
        Get
            Return _Successful
        End Get
        Set(ByVal value As Boolean)
            _Successful = value
        End Set
    End Property

    Private _MemberPlanSold As Boolean
    Public Property MemberPlanSold() As Boolean
        Get
            Return _MemberPlanSold
        End Get
        Set(ByVal value As Boolean)
            _MemberPlanSold = value
        End Set
    End Property

    Private _MemberPlanType As Integer
    Public Property MemberPlanType() As Integer
        Get
            Return _MemberPlanType
        End Get
        Set(ByVal value As Integer)
            _MemberPlanType = value
        End Set
    End Property

End Class
